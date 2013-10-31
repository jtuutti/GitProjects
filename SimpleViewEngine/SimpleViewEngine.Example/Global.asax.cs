using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SimpleViewEngine.Controllers;

namespace SimpleViewEngine.Example
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ControllerBuilder.Current.SetControllerFactory(new HtmlControllerFactory());

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new HtmlViewEngine());
        }
    }
}
