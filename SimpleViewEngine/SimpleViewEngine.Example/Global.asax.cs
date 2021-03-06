﻿using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SimpleViewEngine.Controllers;
using SimpleViewEngine.Example.Serializers;

namespace SimpleViewEngine.Example
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ControllerBuilder.Current.SetControllerFactory(new HtmlControllerFactory());

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new HtmlViewEngine
            {
                AppVersion = "1.0",
                AntiForgeryTokenSupport = true,
                ModelPropertyName = "model",
                BundleSupport = true,
                MinifyHtml = true,
                ModelSerializer = new ModelSerializer()
            });
        }
    }
}
