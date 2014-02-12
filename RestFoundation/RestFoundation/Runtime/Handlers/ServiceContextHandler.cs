// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Web;
using System.Web.Routing;

namespace RestFoundation.Runtime.Handlers
{
    internal sealed class ServiceContextHandler : IServiceContextHandler
    {
        public ServiceContextHandler(IServiceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            Context = context;
        }

        public IServiceContext Context { get; private set; }

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
        }
    }
}
