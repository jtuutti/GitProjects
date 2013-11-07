using System;
using System.Web;
using System.Web.Mvc;
using SimpleViewEngine.Properties;

namespace SimpleViewEngine
{
    /// <summary>
    /// Represents a simple HTML view engine for ASP .NET MVC applications.
    /// </summary>
    public class HtmlViewEngine : VirtualPathProviderViewEngine
    {
        private readonly bool m_cacheHtml;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlViewEngine"/> class.
        /// </summary>
        /// <param name="cacheHtml">
        /// A <see cref="bool"/> indicating whether the view HTML content should be cached
        /// in the ASP .NET server-side cache. File dependencies monitor file changes and
        /// invalidate cache if the content had been changed.
        /// </param>
        public HtmlViewEngine(bool cacheHtml = true)
        {
            if (cacheHtml && HttpRuntime.Cache == null)
            {
                throw new InvalidOperationException(Resources.UnavailableAspCache);
            }

            m_cacheHtml = cacheHtml;

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

            return new HtmlView(filePath, m_cacheHtml);
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

            return new HtmlView(filePath, m_cacheHtml);
        }
    }
}
