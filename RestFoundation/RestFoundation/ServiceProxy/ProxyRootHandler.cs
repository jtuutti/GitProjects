﻿using System.Web;
using System.Web.Routing;

namespace RestFoundation.ServiceProxy
{
    internal sealed class ProxyRootHandler : IRouteHandler, IHttpHandler
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
            context.Response.RedirectToRoutePermanent("ProxyIndex");
        }
    }
}
