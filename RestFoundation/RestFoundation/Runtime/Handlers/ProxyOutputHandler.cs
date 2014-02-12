// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Web;
using System.Web.Routing;

namespace RestFoundation.Runtime.Handlers
{
    internal sealed class ProxyOutputHandler : IRouteHandler, IHttpHandler
    {
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            string contentType = context.Request.Unvalidated.Form["ct"];
            string responseText = context.Server.UrlDecode(context.Request.Unvalidated.Form["txt"]);

            context.Response.Clear();
            context.Response.ContentType = !String.IsNullOrWhiteSpace(contentType) ? contentType : "text/plain";

            if (responseText != null)
            {
                context.Response.Write(responseText);
            }
        }
    }
}
