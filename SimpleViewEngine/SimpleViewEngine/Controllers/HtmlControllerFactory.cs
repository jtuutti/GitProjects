using System;
using System.Web.Mvc;
using System.Web.Routing;
using SimpleViewEngine.Properties;

namespace SimpleViewEngine.Controllers
{
    /// <summary>
    /// Represents a controller factory that creates action-less HTML views.
    /// </summary>
    public class HtmlControllerFactory : DefaultControllerFactory
    {
        /// <summary>
        /// Retrieves the controller instance for the specified request context and controller type.
        /// </summary>
        /// <returns>The controller instance.</returns>
        /// <param name="requestContext">
        /// The context of the HTTP request, which includes the HTTP context and route data.</param>
        /// <param name="controllerType">The type of the controller.</param>
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return new HtmlController();
            }

            IDependencyResolver dependencyResolver = DependencyResolver.Current;

            if (dependencyResolver == null)
            {
                return base.GetControllerInstance(requestContext, controllerType);
            }

            var controller = dependencyResolver.GetService(controllerType) as IController;

            if (controller == null)
            {
                throw new ArgumentException(String.Format(Resources.InvalidControllerType, controllerType.Name), "controllerType");
            }

            return controller;
        }
    }
}
