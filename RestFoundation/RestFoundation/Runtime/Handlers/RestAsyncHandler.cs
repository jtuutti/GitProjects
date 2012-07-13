using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using RestFoundation.Results;

namespace RestFoundation.Runtime.Handlers
{
    public class RestAsyncHandler : IRestAsyncHandler
    {
        private readonly IServiceContext m_serviceContext;
        private readonly IServiceMethodLocator m_serviceMethodLocator;
        private readonly IServiceMethodInvoker m_methodInvoker;
        private readonly IResultFactory m_resultFactory;
        private readonly IResultExecutor m_resultExecutor;

        public RestAsyncHandler(IServiceContext serviceContext,
                                IServiceMethodLocator serviceMethodLocator,
                                IServiceMethodInvoker methodInvoker,
                                IResultFactory resultFactory,
                                IResultExecutor resultExecutor)
        {
            if (serviceContext == null) throw new ArgumentNullException("serviceContext");
            if (serviceMethodLocator == null) throw new ArgumentNullException("serviceMethodLocator");
            if (methodInvoker == null) throw new ArgumentNullException("methodInvoker");
            if (resultFactory == null) throw new ArgumentNullException("resultFactory");
            if (resultExecutor == null) throw new ArgumentNullException("resultExecutor");

            m_serviceContext = serviceContext;
            m_serviceMethodLocator = serviceMethodLocator;
            m_methodInvoker = methodInvoker;
            m_resultFactory = resultFactory;
            m_resultExecutor = resultExecutor;
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
                return Task<IResult>.Factory.StartNew(state =>
                                                      {
                                                          HttpContext.Current = ((HttpArguments) state).Context;
                                                          return new EmptyResult();
                                                      }, new HttpArguments(HttpContext.Current, null))
                                            .ContinueWith(action => cb(action));
            }

            return Task<IResult>.Factory.StartNew(state =>
                                                  {
                                                      HttpContext.Current = ((HttpArguments) state).Context;
                                                      object returnedObj = m_methodInvoker.Invoke(this, serviceMethodData.Service, serviceMethodData.Method);
                                                      return serviceMethodData.Method.ReturnType != typeof(void) ? m_resultFactory.Create(m_serviceContext, returnedObj) : null;
                                                  }, new HttpArguments(HttpContext.Current, serviceMethodData.Method.ReturnType))
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
                throw UnwrapTaskException(task);
            }

            var httpArguments = (HttpArguments) result.AsyncState;
            HttpContext.Current = httpArguments.Context;

            if (!(task.Result is EmptyResult))
            {
                m_resultExecutor.Execute(m_serviceContext, task.Result, httpArguments.MethodReturnType);
            }
        }

        private static Exception UnwrapTaskException(Task<IResult> task)
        {
            AggregateException taskException = task.Exception;

            if (taskException == null)
            {
                return new HttpResponseException(HttpStatusCode.InternalServerError, "HTTP request failed to process");
            }

            return ExceptionUnwrapper.IsDirectResponseException(taskException.InnerException) ? taskException.InnerException : taskException;
        }

        #region HTTP Task Arguments

        private sealed class HttpArguments
        {
            public HttpArguments(HttpContext context, Type methodReturnType)
            {
                Context = context;
                MethodReturnType = methodReturnType;
            }

            public HttpContext Context { get; private set; }
            public Type MethodReturnType { get; private set; }
        }

        #endregion
    }
}
