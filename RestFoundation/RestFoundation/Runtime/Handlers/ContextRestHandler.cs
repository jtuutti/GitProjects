// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Web;
using System.Web.Routing;

namespace RestFoundation.Runtime.Handlers
{
    internal sealed class ContextRestHandler : IRestHandler
    {
        public ContextRestHandler(IServiceContext context)
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

        public string ServiceUrl
        {
            get
            {
                return null;
            }
        }

        public string ServiceContractTypeName
        {
            get
            {
                return null;
            }
        }

        public string UrlTemplate
        {
            get
            {
                return null;
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
