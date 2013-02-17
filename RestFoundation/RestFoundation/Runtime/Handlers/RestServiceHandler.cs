// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using RestFoundation.Behaviors.Attributes;

namespace RestFoundation.Runtime.Handlers
{
    /// <summary>
    /// Represents a REST service handler implementation.
    /// </summary>
    public class RestServiceHandler : HttpTaskAsyncHandler, IRestServiceHandler
    {
        private const int DefaultServiceTimeoutInSeconds = 90;

        private readonly IServiceContext m_serviceContext;
        private readonly IServiceMethodLocator m_methodLocator;
        private readonly IServiceMethodInvoker m_methodInvoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestServiceHandler"/> class.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="methodLocator">The service method locator.</param>
        /// <param name="methodInvoker">The service method invoker.</param>
        public RestServiceHandler(IServiceContext serviceContext, IServiceMethodLocator methodLocator, IServiceMethodInvoker methodInvoker)
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

            m_serviceContext = serviceContext;
            m_methodLocator = methodLocator;
            m_methodInvoker = methodInvoker;

            ServiceTimeout = TimeSpan.FromSeconds(DefaultServiceTimeoutInSeconds);
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
        /// Gets a fully qualified name of the interface type defining the service contract.
        /// </summary>
        public string ServiceContractTypeName { get; protected set; }

        /// <summary>
        /// Gets the service URL.
        /// </summary>
        public string ServiceUrl { get; protected set; }

        /// <summary>
        /// Gets a relative URL template.
        /// </summary>
        public string UrlTemplate { get; protected set; }

        /// <summary>
        /// Gets or sets a value representing the service method execution timeout.
        /// Setting the value to <see cref="TimeSpan.Zero"/> indicates no timeout.
        /// </summary>
        public TimeSpan ServiceTimeout { get; set; }

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

            var routeInitializer = new RestServiceRouteInitializer(this);
            RestServiceRouteInfo routeInfo = routeInitializer.Initialize(requestContext);

            ServiceContractTypeName = routeInfo.ServiceContractTypeName;
            ServiceUrl = routeInfo.ServiceUrl;
            UrlTemplate = routeInfo.UrlTemplate;

            return this;
        }

        /// <summary>
        /// When overridden in a derived class, provides code that handles an asynchronous task.
        /// </summary>
        /// <returns>The asynchronous task.</returns>
        /// <param name="context">The HTTP context.</param>
        public override async Task ProcessRequestAsync(HttpContext context)
        {
            ServiceMethodLocatorData serviceMethodData = m_methodLocator.Locate(this);

            if (serviceMethodData == ServiceMethodLocatorData.Options)
            {
                return;
            }

            TrySetServiceMethodTimeout(serviceMethodData.Method);

            var cancellation = new CancellationTokenSource();
            Task methodTask = m_methodInvoker.Invoke(this, serviceMethodData.Service, serviceMethodData.Method, cancellation.Token);

            if (methodTask.Status == TaskStatus.Created)
            {
                methodTask.Start();
            }

            if (ServiceTimeout.TotalMilliseconds > 0)
            {
                await Task.WhenAny(methodTask, Task.Delay(ServiceTimeout)).ConfigureAwait(false);
            }
            else
            {
                await methodTask.ConfigureAwait(false);
            }

            ValidateTask(methodTask, cancellation);
        }
        
        private static void ValidateTask(Task task, CancellationTokenSource cancellation)
        {
            if (task.IsFaulted)
            {
                throw TaskExceptionUnwrapper.Unwrap(task);
            }

            if (task.Status == TaskStatus.RanToCompletion)
            {
                return;
            }

            try
            {
                cancellation.Cancel();
            }
            catch (Exception)
            {
            }

            throw new HttpResponseException(HttpStatusCode.ServiceUnavailable, RestResources.ServiceTimedOut);
        }

        private void TrySetServiceMethodTimeout(MethodInfo method)
        {
            var timeoutAttribute = Attribute.GetCustomAttribute(method, typeof(ServiceMethodTimeoutAttribute), false) as ServiceMethodTimeoutAttribute;

            if (timeoutAttribute == null)
            {
                return;
            }

            ServiceTimeout = TimeSpan.FromSeconds(timeoutAttribute.ServiceTimeoutInSeconds);
        }
    }
}
