using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using RestFoundation.Results;

namespace RestFoundation.Runtime
{
    public class RestAsyncHandler : IRestAsyncHandler
    {
        private readonly IServiceContext m_serviceContext;
        private readonly ServiceMethodLocator m_serviceMethodLocator;
        private readonly IServiceMethodInvoker m_methodInvoker;
        private readonly IResultFactory m_resultFactory;

        public RestAsyncHandler(IServiceContext serviceContext, ServiceMethodLocator serviceMethodLocator, IServiceMethodInvoker methodInvoker, IResultFactory resultFactory)
        {
            if (serviceContext == null) throw new ArgumentNullException("serviceContext");
            if (serviceMethodLocator == null) throw new ArgumentNullException("serviceMethodLocator");
            if (methodInvoker == null) throw new ArgumentNullException("methodInvoker");
            if (resultFactory == null) throw new ArgumentNullException("resultFactory");

            m_serviceContext = serviceContext;
            m_serviceMethodLocator = serviceMethodLocator;
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

        public string ServiceUrl { get; protected set; }
        public string ServiceContractTypeName { get; protected set; }
        public string UrlTemplate { get; protected set; }

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
            ServiceMethodLocatorData serviceMethodData = m_serviceMethodLocator.Execute(this);

            if (serviceMethodData == ServiceMethodLocatorData.Options)
            {
                return Task<IResult>.Factory.StartNew(() => new EmptyResult()).ContinueWith(action => cb(action));
            }

            var httpArguments = new HttpArguments(HttpContext.Current, serviceMethodData.Method.ReturnType);

            return Task<IResult>.Factory.StartNew(state =>
                                                 {
                                                     HttpContext.Current = ((HttpArguments) state).Context;
                                                     return m_resultFactory.Create(m_serviceContext, m_methodInvoker.Invoke(this, serviceMethodData.Service, serviceMethodData.Method));
                                                 }, httpArguments)
                                                 .ContinueWith(action => cb(action));
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            if (result == null)
            {
                return;
            }

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
            public HttpArguments(HttpContext context, Type methodReturnType)
            {
                if (methodReturnType == null)
                {
                    throw new ArgumentNullException("methodReturnType");
                }

                Context = context;
                MethodReturnType = methodReturnType;
            }

            public HttpContext Context { get; private set; }
            public Type MethodReturnType { get; private set; }
        }

        #endregion
    }
}
