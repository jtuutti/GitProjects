using System;
using System.Web.Routing;
using System.Web;

namespace MvcAlt
{
    public class Global : HttpApplication
    {
        private void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.AddControllerDefinedRoutes("MvcAlt");

            //var frontControllerRoute = new Route("{*_url}", new FrontController());
            //RouteTable.Routes.Add(frontControllerRoute);
        }

        private void Application_End(object sender, EventArgs e)
        {
        }

        private void Application_Error(object sender, EventArgs e)
        {
        }

        private void Session_Start(object sender, EventArgs e)
        {
        }

        private void Session_End(object sender, EventArgs e)
        {
        }
    }
}