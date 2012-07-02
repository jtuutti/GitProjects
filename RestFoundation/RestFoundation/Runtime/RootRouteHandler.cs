using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Routing;
using RestFoundation.Collections.Specialized;
using RestFoundation.DataFormatters;
using RestFoundation.ServiceProxy;

namespace RestFoundation
{
    public class RootRouteHandler : IRouteHandler, IHttpHandler
    {
        private readonly IServiceContext m_serviceContext;
        private readonly IResultFactory m_resultFactory;

        private HttpContextBase m_context;

        public RootRouteHandler(IServiceContext serviceContext, IResultFactory resultFactory)
        {
            if (serviceContext == null) throw new ArgumentNullException("serviceContext");
            if (resultFactory == null) throw new ArgumentNullException("resultFactory");

            m_serviceContext = serviceContext;
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

            m_context = requestContext.HttpContext;

            return this;
        }

        public void ProcessRequest(HttpContext context)
        {
            if (String.Equals(HttpMethod.Options.ToString(), m_context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                m_serviceContext.Response.SetHeader("Allow", "GET, HEAD");
                return;
            }

            if (!String.Equals(HttpMethod.Get.ToString(), m_context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase) &&
                !String.Equals(HttpMethod.Head.ToString(), m_context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed, "HTTP method is not allowed");
            }

            if (String.Equals(HttpMethod.Head.ToString(), m_context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                m_context.Response.SuppressContent = true;
            }

            if (Rest.Active.IsServiceProxyInitialized && IsInBrowser(m_context.Request))
            {
                m_context.Response.Redirect((m_context.Request.ApplicationPath ?? String.Empty).TrimEnd('/') + "/help/index", false);
                return;
            }

            IResult result;
            
            try
            {
                result = m_resultFactory.Create(m_serviceContext, GetOperations());
            }
            catch (HttpResponseException)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden, "The service is configured not to list its contents in the requested format");
            }

            if (result == null)
            {
                return;
            }

            result.Execute(m_serviceContext);
        }

        private static bool IsInBrowser(HttpRequestBase request)
        {
            if (!String.Equals(request.HttpMethod, HttpMethod.Get.ToString(), StringComparison.OrdinalIgnoreCase) &&
                !String.Equals(request.HttpMethod, HttpMethod.Head.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string browser = request.Browser != null ? request.Browser.Browser : null;
            string[] acceptTypes = request.AcceptTypes;

            if (String.IsNullOrWhiteSpace(browser) || acceptTypes == null || acceptTypes.Length == 0)
            {
                return false;
            }

            string acceptedValue = request.QueryString["X-Accept-Override"];

            if (String.IsNullOrEmpty(acceptedValue))
            {
                acceptedValue = request.Headers.Get("Accept");
            }

            var acceptTypeCollection = new AcceptValueCollection(acceptedValue);

            if (acceptTypeCollection.AcceptedNames.Length == 0)
            {
                return false;
            }

            string[] contentTypes = DataFormatterRegistry.GetContentTypes();

            for (int i = 0; i < contentTypes.Length; i++)
            {
                if (String.Equals(contentTypes[i], acceptTypeCollection.GetPreferredName(), StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return acceptTypeCollection.CanAccept("text/html");
        }

        private static Operation[] GetOperations()
        {
            IEnumerable<ProxyOperation> proxyOperations = ProxyOperationGenerator.Generate();

            var operations = new List<Operation>();

            foreach (ProxyOperation proxyOperation in proxyOperations)
            {
                if (proxyOperation.IsIpFiltered)
                {
                    continue;
                }

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
    }
}
