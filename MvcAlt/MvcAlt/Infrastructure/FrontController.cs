using System.Web;
using System.Web.Routing;

namespace MvcAlt.Infrastructure
{
    public class FrontController : IRouteHandler, IHttpHandler
    {
        private readonly ActionInvoker m_invoker;

        public FrontController()
        {
            m_invoker = new ActionInvoker();
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new FrontController();
        }

        public void ProcessRequest(HttpContext context)
        {
            IHttpRequest request = new HttpRequest(context);
            object result = m_invoker.Invoke(request);

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
