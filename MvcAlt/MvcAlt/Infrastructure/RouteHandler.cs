using System.Web;
using System.Web.Routing;

namespace MvcAlt.Infrastructure
{
    public class RouteHandler : IRouteHandler, IHttpHandler
    {
        private readonly IActionMethodInvoker methodInvoker;

        public RouteHandler()
        {
            methodInvoker = new DefaultActionMethodInvoker();
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
            return new RouteHandler();
        }

        public void ProcessRequest(HttpContext context)
        {
            IHttpRequest request = new HttpRequest(context);

            if (request.Verb == HttpVerb.Head)
            {
                context.Response.SuppressContent = true;
            }

            object result = methodInvoker.Invoke(request);

            if (result == null)
            {
                context.Response.StatusCode = 204;
            }
            else
            {
                context.Response.Write(result.ToString());
                context.Response.StatusCode = 200;
            }
        }
    }
}
