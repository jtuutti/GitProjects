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
        private readonly DateTime? m_cacheExpiration;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlViewEngine"/> class.
        /// </summary>
        /// <param name="cacheHtml">
        /// A <see cref="bool"/> indicating whether the view HTML content should be cached
        /// in the ASP .NET server-side cache. The default expiration time is 60 minutes.
        /// File dependencies monitor file changes and invalidate cache if the file content
        /// had been changed.
        /// </param>
        public HtmlViewEngine(bool cacheHtml = true) :
            this(cacheHtml ? TimeSpan.FromMinutes(60) : TimeSpan.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlViewEngine"/> class.
        /// </summary>
        /// <param name="cacheExpiration">
        /// Sets the HTML cache expiration interval. Set this parameter to
        /// <see cref="TimeSpan.Zero"/> to disable the HTML cache.
        /// </param>
        public HtmlViewEngine(TimeSpan cacheExpiration)
        {
            if (cacheExpiration > TimeSpan.Zero)
            {
                if (HttpRuntime.Cache == null)
                {
                    throw new InvalidOperationException(Resources.UnavailableAspCache);
                }

                m_cacheExpiration = DateTime.Now.Add(cacheExpiration);
            }

            ViewLocationFormats = new[] { "~/views/{1}/{0}.html", "~/views/shared/{0}.html" };
            PartialViewLocationFormats = new[] { "~/views/{1}/{0}.partial.html", "~/views/shared/{0}.partial.html" };
            FileExtensions = new [] { "html" };
        }

        /// <summary>
        /// Gets or sets the application version. It can be appended to CSS and JavaScript
        /// links using the <code>:version</code> URL variable.
        /// </summary>
        public string AppVersion { get; set; }

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

            return new HtmlView(filePath, AppVersion, null);
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

            return new HtmlView(filePath, AppVersion, m_cacheExpiration);
        }
    }
}
