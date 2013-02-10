﻿// <copyright>
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
using RestFoundation.Results;

namespace RestFoundation.Runtime.Handlers
{
    /// <summary>
    /// Represents a REST service handler implementation.
    /// </summary>
    public class RestServiceHandler : HttpTaskAsyncHandler, IRestServiceHandler
    {
        private const int DefaultServiceTimeoutInSeconds = 60;
        private const int DefaultResultTimeoutInSeconds = 30;

        private readonly IServiceContext m_serviceContext;
        private readonly IServiceMethodLocator m_methodLocator;
        private readonly IServiceMethodInvoker m_methodInvoker;
        private readonly IResultFactory m_resultFactory;
        private readonly IResultExecutor m_resultExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestServiceHandler"/> class.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="methodLocator">The service method locator.</param>
        /// <param name="methodInvoker">The service method invoker.</param>
        /// <param name="resultFactory">The service method result factory.</param>
        /// <param name="resultExecutor">The service method result executor.</param>
        public RestServiceHandler(IServiceContext serviceContext,
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

            ServiceTimeout = TimeSpan.FromSeconds(DefaultServiceTimeoutInSeconds);
            ResultTimeout = TimeSpan.FromSeconds(DefaultResultTimeoutInSeconds);
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
        /// Gets or sets a value representing the service method execution timeout.
        /// Setting the value to <see cref="TimeSpan.Zero"/> indicates no timeout.
        /// </summary>
        public TimeSpan ServiceTimeout { get; set; }

        /// <summary>
        /// Gets or sets a value representing the service method result execution timeout.
        /// Setting the value to <see cref="TimeSpan.Zero"/> indicates no timeout.
        /// </summary>
        public TimeSpan ResultTimeout { get; set; }

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

            if (requestContext.RouteData == null || requestContext.RouteData.Values == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, RestResources.MissingRouteData);
            }

            if (!RestHttpModule.IsInitialized)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, RestResources.MissingRestHttpModule);
            }

            if (UnvalidatedHandlerRegistry.IsUnvalidated(this))
            {
                requestContext.HttpContext.Items[ServiceRequestValidator.UnvalidatedHandlerKey] = Boolean.TrueString;
            }

            ServiceUrl = (string) requestContext.RouteData.Values[RouteConstants.ServiceUrl];
            ServiceContractTypeName = (string) requestContext.RouteData.Values[RouteConstants.ServiceContractType];
            UrlTemplate = (string) requestContext.RouteData.Values[RouteConstants.UrlTemplate];

            if (String.IsNullOrEmpty(ServiceUrl) || String.IsNullOrEmpty(ServiceContractTypeName) || UrlTemplate == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, RestResources.NotFound);
            }

            return this;
        }

        /// <summary>
        /// When overridden in a derived class, provides code that handles an asynchronous task.
        /// </summary>
        /// <returns>The asynchronous task.</returns>
        /// <param name="context">The HTTP context.</param>
        public override async Task ProcessRequestAsync(HttpContext context)
        {
            m_serviceContext.Request.ResourceBag.ServiceExecutionId = Guid.NewGuid();

            ServiceMethodLocatorData serviceMethodData = m_methodLocator.Locate(this);

            if (serviceMethodData == ServiceMethodLocatorData.Options)
            {
                return;
            }

            LogRequest();

            TrySetServiceMethodTimeout(serviceMethodData.Method);
                
            var invocationTaskCancellation = new CancellationTokenSource();
            Task<object> invocationTask = GenerateMethodInvocationTask(serviceMethodData, context, invocationTaskCancellation);

            if (ServiceTimeout.TotalMilliseconds > 0)
            {
                await Task.WhenAny(invocationTask, Task.Delay(ServiceTimeout)).ConfigureAwait(false);
            }
            else
            {
                await invocationTask.ConfigureAwait(false);
            }

            ValidateTask(invocationTask, invocationTaskCancellation);

            LogResponse();

            if (invocationTask.Result is IAsyncResult)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, RestResources.InvalidIAsyncResultReturned);
            }

            var resultTaskCancellation = new CancellationTokenSource();
            Task resultTask = GenerateResultTask(invocationTask.Result, serviceMethodData, context, resultTaskCancellation);

            if (ResultTimeout.TotalMilliseconds > 0)
            {
                await Task.WhenAny(resultTask, Task.Delay(ResultTimeout)).ConfigureAwait(false);
            }
            else
            {
                await resultTask.ConfigureAwait(false);
            }

            ValidateTask(resultTask, resultTaskCancellation);
        }

        private static void ValidateTask(Task task, CancellationTokenSource cancellation)
        {
            if (task.IsFaulted)
            {
                throw TaskExceptionUnwrapper.Unwrap(task);
            }

            if (task.Status != TaskStatus.RanToCompletion)
            {
                try
                {
                    cancellation.Cancel();
                }
                catch (Exception)
                {
                }

                throw new HttpResponseException(HttpStatusCode.ServiceUnavailable, RestResources.ServiceTimedOut);
            }
        }

        private void TrySetServiceMethodTimeout(MethodInfo method)
        {
            var timeoutAttribute = Attribute.GetCustomAttribute(method, typeof(ServiceMethodTimeoutAttribute), false) as ServiceMethodTimeoutAttribute;

            if (timeoutAttribute == null)
            {
                return;
            }

            ServiceTimeout = TimeSpan.FromSeconds(timeoutAttribute.ServiceTimeoutInSeconds);

            if (timeoutAttribute.ResultTimeoutInSeconds.HasValue)
            {
                ResultTimeout = TimeSpan.FromSeconds(timeoutAttribute.ResultTimeoutInSeconds.Value);
            }
        }

        private Task<object> GenerateMethodInvocationTask(ServiceMethodLocatorData serviceMethodData, HttpContext context, CancellationTokenSource cancellation)
        {
            var invocationTask = Task<object>.Factory.StartNew(ctx =>
            {
                if (ctx != null)
                {
                    HttpContext.Current = (HttpContext) ctx;
                }

                return m_methodInvoker.Invoke(serviceMethodData.Service, serviceMethodData.Method, this); 
            }, context, cancellation.Token);

            return invocationTask;
        }

        private Task GenerateResultTask(object returnedObj, ServiceMethodLocatorData serviceMethodData, HttpContext context, CancellationTokenSource cancellation)
        {
            var resultTask = Task.Factory.StartNew(ctx =>
            {
                if (ctx != null)
                {
                    HttpContext.Current = (HttpContext) ctx;
                }

                ExecuteResult(returnedObj, serviceMethodData.Method.ReturnType);
            }, context, cancellation.Token);

            return resultTask;
        }

        private void ExecuteResult(object returnedObj, Type methodReturnType)
        {
            IResult result = methodReturnType != typeof(void) ? m_resultFactory.Create(returnedObj, MethodReturnTypeUnwrapper.Unwrap(methodReturnType), this) : null;

            if (!(result is EmptyResult))
            {
                m_resultExecutor.Execute(result, methodReturnType, m_serviceContext);
            }
        }

        private void LogResponse()
        {
            if (LogUtility.CanLog)
            {
                LogUtility.LogResponseData(m_serviceContext.GetHttpContext());
            }
        }

        private void LogRequest()
        {
            if (LogUtility.CanLog)
            {
                LogUtility.LogRequestData(m_serviceContext.GetHttpContext());
            }
        }
    }
}