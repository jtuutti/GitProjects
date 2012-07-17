﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Routing;
using RestFoundation.ServiceProxy;

namespace RestFoundation.Runtime.Handlers
{
    /// <summary>
    /// Represents a service root route handler.
    /// </summary>
    public class RootRouteHandler : IRestHandler
    {
        private readonly IServiceContext m_serviceContext;
        private readonly IBrowserDetector m_browserDetector;
        private readonly IResultFactory m_resultFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootRouteHandler"/> class.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="browserDetector">The browser detector.</param>
        /// <param name="resultFactory">The service method result factory.</param>
        public RootRouteHandler(IServiceContext serviceContext, IBrowserDetector browserDetector, IResultFactory resultFactory)
        {
            if (serviceContext == null) throw new ArgumentNullException("serviceContext");
            if (browserDetector == null) throw new ArgumentNullException("browserDetector");
            if (resultFactory == null) throw new ArgumentNullException("resultFactory");

            m_serviceContext = serviceContext;
            m_browserDetector = browserDetector;
            m_resultFactory = resultFactory;
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
        public string ServiceUrl
        {
            get
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Gets a fully qualified name of the interface type defining the service contract.
        /// </summary>
        public string ServiceContractTypeName
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets a relative URL template.
        /// </summary>
        public string UrlTemplate
        {
            get
            {
                throw new NotSupportedException();
            }
        }

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

            if (!HttpResponseModule.IsLoaded(requestContext.HttpContext))
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No HTTP response module found");
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
            if (m_serviceContext.Request.Method == HttpMethod.Options)
            {
                m_serviceContext.Response.SetHeader("Allow", "GET, HEAD");
                return;
            }

            if (m_serviceContext.Request.Method != HttpMethod.Get && m_serviceContext.Request.Method != HttpMethod.Head)
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed, "HTTP method is not allowed");
            }

            if (context != null)
            {
                if (m_serviceContext.Request.Method == HttpMethod.Head)
                {
                    context.Response.SuppressContent = true;
                }

                if (Rest.Active.IsServiceProxyInitialized && !String.IsNullOrWhiteSpace(Rest.Active.ServiceProxyRelativeUrl) &&
                    m_browserDetector.IsBrowserRequest(new HttpRequestWrapper(context.Request)))
                {
                    string rootUrl = String.Concat((context.Request.ApplicationPath ?? String.Empty).TrimEnd('/'), "/", Rest.Active.ServiceProxyRelativeUrl, "/index");
                    context.Response.Redirect(rootUrl, false);

                    return;
                }
            }

            ProcessResult();
        }

        private static Operation[] GetOperations()
        {
            IEnumerable<ProxyOperation> proxyOperations = ProxyOperationGenerator.GetAll();

            var operations = new List<Operation>();

            foreach (ProxyOperation proxyOperation in proxyOperations)
            {
                var sampleUrlParts = proxyOperation.GenerateSampleUrlParts();

                operations.Add(new Operation
                {
                    RelativeUrlTemplate = proxyOperation.UrlTempate,
                    Description = proxyOperation.Description,
                    HttpMethod = proxyOperation.HttpMethod.ToString().ToUpperInvariant(),
                    SampleUrl = sampleUrlParts != null ? String.Concat(sampleUrlParts.Item1, sampleUrlParts.Item2) : String.Empty
                });
            }

            return operations.ToArray();
        }

        private void ProcessResult()
        {
            IResult result;

            try
            {
                result = m_resultFactory.Create(GetOperations(), this);
            }
            catch (HttpResponseException)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden, "The service is configured not to list its contents in the requested format");
            }

            if (result != null)
            {
                result.Execute(m_serviceContext);
            }
        }
    }
}