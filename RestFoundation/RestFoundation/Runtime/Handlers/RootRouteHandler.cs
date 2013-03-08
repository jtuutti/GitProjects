// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Routing;
using RestFoundation.Configuration;
using RestFoundation.Results;
using RestFoundation.ServiceProxy;

namespace RestFoundation.Runtime.Handlers
{
    /// <summary>
    /// Represents a service root route handler.
    /// </summary>
    public class RootRouteHandler : IServiceContextHandler
    {
        private const char Slash = '/';

        private readonly IServiceContext m_serviceContext;
        private readonly IContentNegotiator m_contentNegotiator;
        private readonly IResultWrapper m_resultWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootRouteHandler"/> class.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="contentNegotiator">The content negotiator.</param>
        /// <param name="resultWrapper">The service method result wrapper.</param>
        public RootRouteHandler(IServiceContext serviceContext, IContentNegotiator contentNegotiator, IResultWrapper resultWrapper)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException("serviceContext");
            }

            if (contentNegotiator == null)
            {
                throw new ArgumentNullException("contentNegotiator");
            }

            if (resultWrapper == null)
            {
                throw new ArgumentNullException("resultWrapper");
            }

            m_serviceContext = serviceContext;
            m_contentNegotiator = contentNegotiator;
            m_resultWrapper = resultWrapper;
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

            if (!RestHttpModule.IsInitialized)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, Resources.Global.MissingRestHttpModule);
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
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed, Resources.Global.DisallowedHttpMethod);
            }

            if (context != null)
            {
                if (m_serviceContext.Request.Method == HttpMethod.Head)
                {
                    context.Response.SuppressContent = true;
                }

                if (m_contentNegotiator.IsBrowserRequest(m_serviceContext.Request) && ProcessRequestForBrowser(context))
                {
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

        private bool ProcessRequestForBrowser(HttpContext context)
        {
            RestOptions options = Rest.Configuration.Options;

            if (!String.IsNullOrWhiteSpace(options.IndexPageRelativeUrl))
            {
                try
                {
                    context.Server.Transfer(options.IndexPageRelativeUrl, false);
                    return true;
                }
                catch (Exception)
                {
                    throw new InvalidOperationException(Resources.Global.UnableToLoadIndexPage);
                }
            }

            if (!options.IsServiceProxyInitialized || String.IsNullOrWhiteSpace(options.ServiceProxyRelativeUrl))
            {
                return false;
            }

            context.Response.Redirect(GenerateProxyUrl(options), false);
            return true;
        }

        private string GenerateProxyUrl(RestOptions options)
        {
            string serviceUrl = m_serviceContext.Request.Url.GetLeftPart(UriPartial.Path);

            return String.Concat(serviceUrl.TrimEnd(Slash), Slash, options.ServiceProxyRelativeUrl, Slash, "index");
        }

        private void ProcessResult()
        {
            IResult result;

            try
            {
                result = m_resultWrapper.Wrap(this, GetOperations(), typeof(Operation[]));
            }
            catch (HttpResponseException)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden, Resources.Global.UnsupportedRequestedFormat);
            }

            if (result != null)
            {
                result.Execute(m_serviceContext);
            }
        }
    }
}
