using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using RestFoundation.Acl;
using RestFoundation.Results;

namespace RestFoundation.Runtime
{
    public class RestAsyncHandler : IRestAsyncHandler
    {
        private readonly IServiceContext m_serviceContext;
        private readonly IServiceFactory m_serviceFactory;
        private readonly IResultFactory m_resultFactory;
        private readonly IServiceMethodInvoker m_methodInvoker;

        public RestAsyncHandler(IServiceContext serviceContext, IServiceFactory serviceFactory, IServiceMethodInvoker methodInvoker, IResultFactory resultFactory)
        {
            if (serviceContext == null) throw new ArgumentNullException("serviceContext");
            if (serviceFactory == null) throw new ArgumentNullException("serviceFactory");
            if (methodInvoker == null) throw new ArgumentNullException("methodInvoker");
            if (resultFactory == null) throw new ArgumentNullException("resultFactory");

            m_serviceContext = serviceContext;
            m_serviceFactory = serviceFactory;
            m_methodInvoker = methodInvoker;
            m_resultFactory = resultFactory;
        }

        public IServiceContext Context
        {
            get
            {
                return m_serviceContext;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        protected string ServiceUrl { get; set; }
        protected string ServiceContractTypeName { get; set; }
        protected string UrlTemplate { get; set; }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (requestContext == null) throw new ArgumentNullException("requestContext");

            if (UnvalidatedHandlerRegistry.IsUnvalidated(this))
            {
                requestContext.HttpContext.Items[ServiceRequestValidator.UnvalidatedHandlerKey] = Boolean.TrueString;
            }

            if (requestContext.RouteData == null || requestContext.RouteData.Values == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No route data found");
            }

            ServiceUrl = (string) requestContext.RouteData.Values[RouteConstants.ServiceUrl];
            ServiceContractTypeName = (string) requestContext.RouteData.Values[RouteConstants.ServiceContractType];
            UrlTemplate = (string) requestContext.RouteData.Values[RouteConstants.UrlTemplate];

            if (String.IsNullOrEmpty(ServiceUrl) || String.IsNullOrEmpty(ServiceContractTypeName) || UrlTemplate == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "Not Found");
            }

            return this;
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new NotSupportedException("Async route handler does not support synchronous requests");
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            Type serviceContractType = ServiceContractTypeRegistry.GetType(ServiceContractTypeName);

            if (serviceContractType == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, String.Format("Service contract of type '{0}' could not be determined", ServiceContractTypeName));
            }

            HttpMethod httpMethod = m_serviceContext.Request.Method;

            if (httpMethod == HttpMethod.Options)
            {
                HashSet<HttpMethod> allowedHttpMethods = HttpMethodRegistry.GetHttpMethods(new RouteMetadata(serviceContractType.AssemblyQualifiedName, UrlTemplate));
                m_serviceContext.Response.SetHeader("Allow", String.Join(", ", allowedHttpMethods.Select(m => m.ToString().ToUpperInvariant()).OrderBy(m => m)));

                return Task<IResult>.Factory.StartNew(() => new EmptyResult()).ContinueWith(action => cb(action));
            }

            if (httpMethod == HttpMethod.Head && context != null)
            {
                context.Response.SuppressContent = true;
            }

            object service = m_serviceFactory.Create(m_serviceContext, serviceContractType);

            if (service == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, String.Format("Service with contract of type '{0}' could not be created", ServiceContractTypeName));
            }

            ValidateAclAttribute acl;
            OutputCacheAttribute cache;
            MethodInfo method = ServiceMethodRegistry.GetMethod(new ServiceMetadata(serviceContractType, ServiceUrl), UrlTemplate, httpMethod, out acl, out cache);

            if (acl != null && context != null)
            {
                AclValidator.Validate(context, acl.SectionName);
            }

            var httpArguments = new HttpArguments(HttpContext.Current, httpMethod, cache, method.ReturnType);

            return Task<IResult>.Factory.StartNew(state =>
                                                 {
                                                     HttpContext.Current = ((HttpArguments) state).Context;
                                                     return m_resultFactory.Create(m_serviceContext, m_methodInvoker.Invoke(this, service, method));
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

            if (!(task.Result is EmptyResult))
            {
                ProcessResult(task.Result, (HttpArguments) result.AsyncState);
            }
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

        private void ProcessResult(IResult result, HttpArguments httpArguments)
        {
            HttpContext.Current = httpArguments.Context;

            if (result != null)
            {
                if ((httpArguments.Method == HttpMethod.Get || httpArguments.Method == HttpMethod.Head) && httpArguments.Cache != null && httpArguments.Context != null)
                {
                    using (var page = new OutputCachedPage(httpArguments.Cache.CacheSettings))
                    {
                        page.ProcessRequest(httpArguments.Context);
                    }
                }

                result.Execute(m_serviceContext);
            }
            else if (m_serviceContext.Response.GetStatusCode() == HttpStatusCode.OK && httpArguments.MethodReturnType == typeof(void))
            {
                m_serviceContext.Response.SetStatus(HttpStatusCode.NoContent, "No Content");
            }
        }

        #region HTTP Task Arguments

        private sealed class HttpArguments
        {
            public HttpArguments(HttpContext context, HttpMethod method, OutputCacheAttribute cache, Type methodReturnType)
            {
                if (methodReturnType == null)
                {
                    throw new ArgumentNullException("methodReturnType");
                }

                Context = context;
                Method = method;
                Cache = cache;
                MethodReturnType = methodReturnType;
            }

            public HttpContext Context { get; private set; }
            public HttpMethod Method { get; private set; }
            public OutputCacheAttribute Cache { get; private set; }
            public Type MethodReturnType { get; private set; }
        }

        #endregion
    }
}
