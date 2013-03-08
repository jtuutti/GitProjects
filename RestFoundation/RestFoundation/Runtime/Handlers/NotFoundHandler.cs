// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System.Net;
using System.Web;
using System.Web.Routing;

namespace RestFoundation.Runtime.Handlers
{
    internal sealed class NotFoundHandler : IRouteHandler, IHttpHandler
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
            throw new HttpResponseException(HttpStatusCode.NotFound, Resources.Global.NotFound);
        }
    }
}
