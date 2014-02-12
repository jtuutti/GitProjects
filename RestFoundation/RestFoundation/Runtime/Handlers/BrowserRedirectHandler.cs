// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Net;
using System.Web;
using System.Web.Routing;

namespace RestFoundation.Runtime.Handlers
{
    internal sealed class BrowserRedirectHandler : IRouteHandler, IHttpHandler
    {
        private string m_webPageUrl;

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (requestContext == null)
            {
                throw new ArgumentNullException("requestContext");
            }

            m_webPageUrl = requestContext.RouteData.Values[ServiceCallConstants.WebPageUrl] as string;

            return this;
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (String.IsNullOrWhiteSpace(m_webPageUrl))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, Resources.Global.NotFound);
            }

            context.Response.Redirect(m_webPageUrl);
        }
    }
}
