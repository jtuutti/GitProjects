using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Routing;
using RestFoundation.ServiceProxy;

namespace RestFoundation
{
    public class RootRouteHandler : IRouteHandler, IHttpHandler
    {
        private readonly IServiceContext m_serviceContext;
        private readonly IBrowserDetector m_browserDetector;
        private readonly IResultFactory m_resultFactory;

        public RootRouteHandler(IServiceContext serviceContext, IBrowserDetector browserDetector, IResultFactory resultFactory)
        {
            if (serviceContext == null) throw new ArgumentNullException("serviceContext");
            if (browserDetector == null) throw new ArgumentNullException("browserDetector");
            if (resultFactory == null) throw new ArgumentNullException("resultFactory");

            m_serviceContext = serviceContext;
            m_browserDetector = browserDetector;
            m_resultFactory = resultFactory;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (requestContext == null) throw new ArgumentNullException("requestContext");

            return this;
        }

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

                if (Rest.Active.IsServiceProxyInitialized && m_browserDetector.IsBrowserRequest(new HttpRequestWrapper(context.Request)))
                {
                    context.Response.Redirect((context.Request.ApplicationPath ?? String.Empty).TrimEnd('/') + "/help/index", false);
                    return;
                }
            }

            ProcessResult();
        }

        private static Operation[] GetOperations()
        {
            IEnumerable<ProxyOperation> proxyOperations = ProxyOperationGenerator.Generate();

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
                result = m_resultFactory.Create(m_serviceContext, GetOperations());
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
