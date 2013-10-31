using System;
using System.Web.Mvc;

namespace SimpleViewEngine
{
    /// <summary>
    /// Represents a simple HTML view engine for ASP .NET MVC applications.
    /// </summary>
    public class HtmlViewEngine : VirtualPathProviderViewEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlViewEngine"/> class.
        /// </summary>
        public HtmlViewEngine()
        {
            ViewLocationFormats = new[] { "~/views/{1}/{0}.html", "~/views/shared/{0}.html" };
            PartialViewLocationFormats = new[] { "~/views/{1}/{0}.partial.html", "~/views/shared/{0}.partial.html" };
            FileExtensions = new [] { "html" };
        }

        /// <summary>
        /// Creates the specified partial view by using the specified controller context.
        /// </summary>
        /// <returns>
        /// A reference to the partial view.
        /// </returns>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="partialPath">The partial path for the new partial view.</param>
        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }

            if (String.IsNullOrEmpty(partialPath))
            {
                throw new ArgumentNullException("partialPath");
            }

            var filePath = controllerContext.HttpContext.Server.MapPath(partialPath);

            return new HtmlView(filePath);
        }

        /// <summary>
        /// Creates the specified view by using the controller context, path of the view,
        /// and path of the master view.
        /// </summary>
        /// <returns>
        /// A reference to the view.
        /// </returns>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="viewPath">The path of the view.</param>
        /// <param name="masterPath">The path of the master view.</param>
        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }

            if (String.IsNullOrEmpty(viewPath))
            {
                throw new ArgumentNullException("viewPath");
            }

            var filePath = controllerContext.HttpContext.Server.MapPath(viewPath);

            return new HtmlView(filePath);
        }
    }
}
