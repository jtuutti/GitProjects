using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using RestFoundation.Runtime;

namespace RestFoundation
{
    public sealed class RestAsyncHandler : IRouteHandler, IHttpAsyncHandler
    {
        private RouteValueDictionary m_routeValues;
        private IServiceFactory m_serviceFactory;
        private IResultFactory m_resultFactory;
        private IActionMethodInvoker m_methodInvoker;

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (requestContext == null) throw new ArgumentNullException("requestContext");

            if (UnvalidatedHandlerRegistry.IsUnvalidated(this))
            {
                requestContext.HttpContext.Items[ServiceRequestValidator.UnvalidatedHandlerKey] = Boolean.TrueString;
            }

            m_routeValues = requestContext.RouteData.Values;
            m_serviceFactory = ObjectActivator.Create<IServiceFactory>();
            m_resultFactory = ObjectActivator.Create<IResultFactory>();
            m_methodInvoker = ObjectActivator.Create<IActionMethodInvoker>();

            return this;
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new NotSupportedException("Async route handler does not support synchronous requests");
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var serviceContractTypeName = (string) m_routeValues[RouteConstants.ServiceContractType];
            var urlTemplate = (string) m_routeValues[RouteConstants.UrlTemplate];

            Type serviceContractType = ServiceContractTypeRegistry.GetType(serviceContractTypeName);

            HashSet<HttpMethod> allowedHttpMethods = HttpMethodRegistry.GetHttpMethods(new RouteMetadata(serviceContractType.AssemblyQualifiedName, urlTemplate));
            HttpMethod httpMethod = context.GetOverriddenHttpMethod();

            if (httpMethod == HttpMethod.Options)
            {
                context.AppendAllowHeader(allowedHttpMethods);
                return Task<IResult>.Factory.StartNew(() => Result.Ok).ContinueWith(action => cb(action));
            }

            if (!allowedHttpMethods.Contains(httpMethod))
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed, "HTTP method is not allowed");
            }

            if (httpMethod == HttpMethod.Head)
            {
                context.Response.SuppressContent = true;
            }

            object service = m_serviceFactory.Create(serviceContractType);

            OutputCacheAttribute cache;
            MethodInfo actionMethod = ActionMethodRegistry.GetActionMethod(serviceContractType, urlTemplate, httpMethod, out cache);

            var httpArguments = new HttpArguments(HttpContext.Current, httpMethod, cache, actionMethod.ReturnType);

            return Task<IResult>.Factory.StartNew(state =>
                                                 {
                                                     HttpContext.Current = ((HttpArguments) state).Context;
                                                     return m_resultFactory.Create(m_methodInvoker.Invoke(this, service, actionMethod));
                                                 }, httpArguments)
                                        .ContinueWith(action => cb(action));
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            var task = (Task<IResult>) result;

            if (task.IsFaulted && task.Exception != null)
            {
                throw UnwrapFaultException(task);
            }

            ProcessResult(task.Result, (HttpArguments) result.AsyncState);
        }

        private static Exception UnwrapFaultException(Task<IResult> task)
        {
            AggregateException taskException = task.Exception;

            if (taskException == null)
            {
                return new HttpResponseException(HttpStatusCode.InternalServerError, "HTTP request failed to process");
            }

            if (taskException.InnerException is HttpResponseException || taskException.InnerException is HttpRequestValidationException)
            {
                return taskException.InnerException;
            }

            return taskException;
        }

        private static void ProcessResult(IResult result, HttpArguments httpArguments)
        {
            HttpContext.Current = httpArguments.Context;

            if (result != null)
            {
                if ((httpArguments.Method == HttpMethod.Get || httpArguments.Method == HttpMethod.Head) && httpArguments.Cache != null)
                {
                    using (var page = new OutputCachedPage(httpArguments.Cache.CacheSettings))
                    {
                        page.ProcessRequest(httpArguments.Context);
                    }
                }

                result.Execute();
            }
            else
            {
               httpArguments.Context.SetActionMethodResponseStatus(httpArguments.ActionMethodReturnType);
            }
        }

        #region HTTP Task Arguments

        private sealed class HttpArguments
        {
            public HttpArguments(HttpContext context, HttpMethod method, OutputCacheAttribute cache, Type actionMethodReturnType)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }

                if (actionMethodReturnType == null)
                {
                    throw new ArgumentNullException("actionMethodReturnType");
                }

                Context = context;
                Method = method;
                Cache = cache;
                ActionMethodReturnType = actionMethodReturnType;
            }

            public HttpContext Context { get; private set; }
            public HttpMethod Method { get; private set; }
            public OutputCacheAttribute Cache { get; private set; }
            public Type ActionMethodReturnType { get; private set; }
        }

        #endregion
    }
}
