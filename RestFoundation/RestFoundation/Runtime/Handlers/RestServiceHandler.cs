﻿// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using RestFoundation.Behaviors;

namespace RestFoundation.Runtime.Handlers
{
    /// <summary>
    /// Represents a REST service handler implementation.
    /// </summary>
    public class RestServiceHandler : HttpTaskAsyncHandler, IRestServiceHandler
    {
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

            ServiceAsyncTimeout = TimeSpan.Zero;
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
        /// Gets or sets a value representing a timeout for an asynchronous task returned by a
        /// service method. Setting the value to <see cref="TimeSpan.Zero"/> indicates no timeout.
        /// </summary>
        public TimeSpan ServiceAsyncTimeout { get; set; }

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

            try
            {
                m_serviceContext.Response.CancellationTokenSource = new CancellationTokenSource();

                TrySetServiceMethodTimeout(serviceMethodData.Method);

                Task methodTask = m_methodInvoker.InvokeAsync(this, serviceMethodData.Service, serviceMethodData.Method);

                if (ServiceAsyncTimeout.TotalMilliseconds > 0)
                {
                    var delayCancellation = new CancellationTokenSource();
                    Task delayTask = Task.Delay(ServiceAsyncTimeout, delayCancellation.Token);

                    if (await Task.WhenAny(methodTask, delayTask) == delayTask)
                    {
                        SafeTaskCancel(Context.Response.CancellationTokenSource);
                        throw new HttpResponseException(HttpStatusCode.ServiceUnavailable, Resources.Global.ServiceTimedOut);
                    }

                    SafeTaskCancel(delayCancellation);
                    ValidateTask(methodTask);
                }
                else
                {
                    await methodTask;
                }
            }
            catch (Exception ex)
            {
                Exception unwrappedException = ExceptionUnwrapper.Unwrap(ex);

                if (ExceptionUnwrapper.IsDirectResponseException(unwrappedException))
                {
                    if (ex == unwrappedException)
                    {
                        throw;
                    }

                    throw unwrappedException;
                }

                try
                {
                    IServiceExceptionHandler exceptionHandler = ServiceExceptionHandlerRegistry.GetExceptionHandler(this);

                    if (exceptionHandler == null ||
                        exceptionHandler.Handle(Context, serviceMethodData.Service, serviceMethodData.Method, unwrappedException) != ExceptionAction.Handle)
                    {
                        throw new ServiceRuntimeException(unwrappedException);
                    }
                }
                catch (HttpResponseException)
                {
                    throw;
                }
                catch (ServiceRuntimeException)
                {
                    throw;
                }
                catch (Exception innerEx)
                {
                    throw new ServiceRuntimeException(ExceptionUnwrapper.Unwrap(innerEx), ex);
                }
            }
            finally
            {
                TryDisposeService(serviceMethodData.Service);
            }
        }

        private static void SafeTaskCancel(CancellationTokenSource cancellation)
        {
            try
            {
                if (cancellation != null && !cancellation.IsCancellationRequested)
                {
                    cancellation.Cancel();
                }
            }
            catch (Exception)
            {
            }
        }

        private static void ValidateTask(Task methodTask)
        {
            if (methodTask.IsFaulted)
            {
                throw TaskExceptionUnwrapper.Unwrap(methodTask);
            }
        }

        private static void TryDisposeService(object service)
        {
            var disposableService = service as IDisposable;

            if (disposableService != null)
            {
                disposableService.Dispose();
            }
        }

        private void TrySetServiceMethodTimeout(MethodInfo method)
        {
            var timeoutAttribute = method.GetCustomAttribute<AsyncTimeoutAttribute>(false);

            if (timeoutAttribute == null)
            {
                return;
            }

            ServiceAsyncTimeout = TimeSpan.FromSeconds(timeoutAttribute.TimeoutInSeconds);
        }
    }
}
