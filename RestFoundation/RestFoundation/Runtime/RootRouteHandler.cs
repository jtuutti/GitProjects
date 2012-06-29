using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Routing;
using RestFoundation.Collections.Specialized;
using RestFoundation.DataFormatters;
using RestFoundation.Runtime;
using RestFoundation.ServiceProxy;
using HttpRequest = System.Web.HttpRequest;

namespace RestFoundation
{
    public class RootRouteHandler : IRouteHandler, IHttpHandler
    {
        private IResultFactory m_resultFactory;

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            m_resultFactory = Rest.Active.CreateObject<IResultFactory>();

            return this;
        }

        public void ProcessRequest(HttpContext context)
        {
            if (String.Equals(HttpMethod.Options.ToString(), context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                context.AppendAllowHeader(new[] { HttpMethod.Get, HttpMethod.Head });
                return;
            }

            if (!String.Equals(HttpMethod.Get.ToString(), context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase) &&
                !String.Equals(HttpMethod.Head.ToString(), context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed, "HTTP method is not allowed");
            }

            if (String.Equals(HttpMethod.Head.ToString(), context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.SuppressContent = true;
            }

            if (Rest.Active.IsServiceProxyInitialized && IsInBrowser(context.Request))
            {
                context.Response.Redirect((context.Request.ApplicationPath ?? String.Empty).TrimEnd('/') + "/index.aspx", false);
                return;
            }

            IResult result;
            
            try
            {
                result = m_resultFactory.Create(GetOperations());
            }
            catch (HttpResponseException)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden, "The service is configured not to list its contents in the requested format");
            }

            if (result == null)
            {
                return;
            }

            result.Execute();
        }

        private static bool IsInBrowser(HttpRequest request)
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

            string[] contentTypes = DataFormatterRegistry.GetContentTypesToFormat();

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
            IEnumerable<ProxyOperation> endPoints = ProxyOperationGenerator.Generate();

            var operations = new List<Operation>();

            foreach (ProxyOperation endPoint in endPoints)
            {
                operations.Add(new Operation
                {
                    RelativeUrlTemplate = endPoint.UrlTempate,
                    Description = endPoint.Description,
                    HttpMethod = endPoint.HttpMethod.ToString().ToUpperInvariant()
                });
            }

            return operations.ToArray();
        }
    }
}
