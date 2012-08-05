using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using RestFoundation.Results;

namespace RestFoundation.Runtime.Handlers
{
    /// <summary>
    /// Represents an asynchronous REST service handler implementation that can be
    /// cancelled by an async timeout.
    /// </summary>
    public class RestAsyncCancellableHandler : IRestAsyncHandler
    {
        private const string ServiceMethodCancellationKey = "_cancellation";

        private readonly IServiceContext m_serviceContext;
        private readonly IServiceMethodLocator m_methodLocator;
        private readonly IServiceMethodInvoker m_methodInvoker;
        private readonly IResultFactory m_resultFactory;
        private readonly IResultExecutor m_resultExecutor;

        private readonly object m_syncRoot = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="RestAsyncCancellableHandler"/> class.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="methodLocator">The service method locator.</param>
        /// <param name="methodInvoker">The service method invoker.</param>
        /// <param name="resultFactory">The service method result factory.</param>
        /// <param name="resultExecutor">The service method result executor.</param>
        public RestAsyncCancellableHandler(IServiceContext serviceContext,
                                           IServiceMethodLocator methodLocator,
                                           IServiceMethodInvoker methodInvoker,
                                           IResultFactory resultFactory,
                                           IResultExecutor resultExecutor)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException("serviceContext");
            }

            if (methodLocator == null)
            {
                throw new ArgumentNullException("methodLocator");
            }

            if (methodInvoker == null)
            {
                throw new ArgumentNullException("methodInvoker");
            }

            if (resultFactory == null)
            {
                throw new ArgumentNullException("resultFactory");
            }

            if (resultExecutor == null)
            {
                throw new ArgumentNullException("resultExecutor");
            }

            m_serviceContext = serviceContext;
            m_methodLocator = methodLocator;
            m_methodInvoker = methodInvoker;
            m_resultFactory = resultFactory;
            m_resultExecutor = resultExecutor;
        }

        /// <summary>
        /// Gets the service context.
        /// </summary>
        public IServiceContext Context
        {
            get
            {
                return m_serviceContext;
            }
        }

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.
        /// </returns>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the asynchronous service method timeout.
        /// It may take some extra time to stop the pending operation.
        /// </summary>
        public TimeSpan AsyncTimeout { get; set; }

        /// <summary>
        /// Gets the service URL.
        /// </summary>
        public string ServiceUrl { get; protected set; }

        /// <summary>
        /// Gets a fully qualified name of the interface type defining the service contract.
        /// </summary>
        public string ServiceContractTypeName { get; protected set; }

        /// <summary>
        /// Gets a relative URL template.
        /// </summary>
        public string UrlTemplate { get; protected set; }

        /// <summary>
        /// Provides the object that processes the request.
        /// </summary>
        /// <returns>
        /// An object that processes the request.
        /// </returns>
        /// <param name="requestContext">An object that encapsulates information about the request.</param>
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (requestContext == null)
            {
                throw new ArgumentNullException("requestContext");
            }

            if (UnvalidatedHandlerRegistry.IsUnvalidated(this))
            {
                requestContext.HttpContext.Items[ServiceRequestValidator.UnvalidatedHandlerKey] = Boolean.TrueString;
            }

            if (requestContext.RouteData == null || requestContext.RouteData.Values == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No route data found");
            }

            if (!RestHttpModule.IsInitialized)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No REST HTTP module found");
            }

            ServiceUrl = (string) requestContext.RouteData.Values[RouteConstants.ServiceUrl];
            ServiceContractTypeName = (string) requestContext.RouteData.Values[RouteConstants.ServiceContractType];
            UrlTemplate = (string) requestContext.RouteData.Values[RouteConstants.UrlTemplate];

            if (String.IsNullOrEmpty(ServiceUrl) || String.IsNullOrEmpty(ServiceContractTypeName) || UrlTemplate == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "Not Found");
            }

            if (AsyncTimeout == TimeSpan.Zero)
            {
                AsyncTimeout = m_serviceContext.ServiceTimeout;
            }

            return this;
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response,
        /// Session, and Server) used to service HTTP requests.
        /// </param>
        public void ProcessRequest(HttpContext context)
        {
            throw new NotSupportedException("Async route handler does not support synchronous requests");
        }

        /// <summary>
        /// Initiates an asynchronous call to the HTTP handler.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.IAsyncResult"/> that contains information about the status of the process.
        /// </returns>
        /// <param name="context">
        /// An <see cref="T:System.Web.HttpContext"/> object that provides references to intrinsic server objects (for example, Request, Response,
        /// Session, and Server) used to service HTTP requests. </param><param name="cb">The <see cref="T:System.AsyncCallback"/> to call when the
        /// asynchronous method call is complete. If <paramref name="cb"/> is null, the delegate is not called.
        /// </param>
        /// <param name="extraData">Any extra data needed to process the request.</param>
        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            ServiceMethodLocatorData serviceMethodData = m_methodLocator.Locate(this);

            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                args.SetObserved();
            };

            if (serviceMethodData == ServiceMethodLocatorData.Options)
            {
                return Task<IResult>.Factory.StartNew(ReturnEmptyResult, new HttpArguments(HttpContext.Current, null))
                                            .ContinueWith(action => cb(action));
            }

            return Task<IResult>.Factory.StartNew(ExecuteServiceMethod, new HttpArguments(HttpContext.Current, serviceMethodData), TaskCreationOptions.LongRunning)
                                        .ContinueWith(action => cb(action));
        }

        /// <summary>
        /// Provides an asynchronous process End method when the process ends.
        /// </summary>
        /// <param name="result">An <see cref="T:System.IAsyncResult"/> that contains information about the status of the process.</param>
        public void EndProcessRequest(IAsyncResult result)
        {
            if (result == null)
            {
                return;
            }

            var httpArguments = (HttpArguments) result.AsyncState;
            HttpContext.Current = httpArguments.Context;

            lock (m_syncRoot)
            {
                CancelMethodIfNecessary(httpArguments);
            }

            var task = (Task<IResult>) result;

            if (task.IsFaulted && task.Exception != null)
            {
                throw UnwrapTaskException(task);
            }

            if (!(task.Result is EmptyResult))
            {
                m_resultExecutor.Execute(task.Result, httpArguments.ServiceMethodData.Method.ReturnType, m_serviceContext);
            }
        }

        private static void CancelMethodIfNecessary(HttpArguments httpArguments)
        {
            var cancellation = httpArguments.Context.Items[ServiceMethodCancellationKey] as CancellationOperation;

            if (cancellation == null)
            {
                return;
            }

            try
            {
                if (cancellation.IsCancelled)
                {
                    throw new HttpResponseException(HttpStatusCode.ServiceUnavailable, "Service timed out");
                }
            }
            finally
            {
                cancellation.Dispose();
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

        private static IResult ReturnEmptyResult(object state)
        {
            HttpContext.Current = ((HttpArguments) state).Context;

            return new EmptyResult();
        }

        private IResult ExecuteServiceMethod(object state)
        {
            var httpArguments = (HttpArguments) state;

            lock (m_syncRoot)
            {
                httpArguments.Context.Items[ServiceMethodCancellationKey] = new CancellationOperation(Thread.CurrentThread, Convert.ToInt32(AsyncTimeout.TotalMilliseconds));
            }

            HttpContext.Current = httpArguments.Context;
            object returnedObj = m_methodInvoker.Invoke(httpArguments.ServiceMethodData.Method, httpArguments.ServiceMethodData.Service, this);

            return httpArguments.ServiceMethodData.Method.ReturnType != typeof(void) ? m_resultFactory.Create(returnedObj, this) : null;
        }
    }
}
