﻿using System;
using System.Net;
using System.Web;
using System.Web.Routing;
using RestFoundation.Results;

namespace RestFoundation.Runtime.Handlers
{
    /// <summary>
    /// Represents an synchronous REST service handler implementation.
    /// </summary>
    public class RestHandler : IRestHandler
    {
        private readonly IServiceContext m_serviceContext;
        private readonly IServiceMethodLocator m_methodLocator;
        private readonly IServiceMethodInvoker m_methodInvoker;
        private readonly IResultFactory m_resultFactory;
        private readonly IResultExecutor m_resultExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestAsyncHandler"/> class.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="methodLocator">The service method locator.</param>
        /// <param name="methodInvoker">The service method invoker.</param>
        /// <param name="resultFactory">The service method result factory.</param>
        /// <param name="resultExecutor">The service method result executor.</param>
        public RestHandler(IServiceContext serviceContext,
                           IServiceMethodLocator methodLocator,
                           IServiceMethodInvoker methodInvoker,
                           IResultFactory resultFactory,
                           IResultExecutor resultExecutor)
        {
            if (serviceContext == null) throw new ArgumentNullException("serviceContext");
            if (methodLocator == null) throw new ArgumentNullException("methodLocator");
            if (methodInvoker == null) throw new ArgumentNullException("methodInvoker");
            if (resultFactory == null) throw new ArgumentNullException("resultFactory");
            if (resultExecutor == null) throw new ArgumentNullException("resultExecutor");

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
            if (requestContext == null) throw new ArgumentNullException("requestContext");

            if (requestContext.RouteData == null || requestContext.RouteData.Values == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No route data found");
            }

            if (!HttpResponseModule.IsLoaded(requestContext.HttpContext))
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No HTTP response module found");
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
                throw new HttpResponseException(HttpStatusCode.NotFound, "Not Found");
            }

            return this;
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/>
        /// interface.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects
        /// (for example, Request, Response, Session, and Server) used to service HTTP requests.
        /// </param>
        public void ProcessRequest(HttpContext context)
        {
            ServiceMethodLocatorData serviceMethodData = m_methodLocator.Locate(this);

            if (serviceMethodData == ServiceMethodLocatorData.Options)
            {
                return;
            }

            object returnedObj = m_methodInvoker.Invoke(serviceMethodData.Method, serviceMethodData.Service, this);
            IResult result = serviceMethodData.Method.ReturnType != typeof(void) ? m_resultFactory.Create(returnedObj, this) : null;

            if (!(result is EmptyResult))
            {
                m_resultExecutor.Execute(result, serviceMethodData.Method.ReturnType, m_serviceContext);
            }
        }
    }
}