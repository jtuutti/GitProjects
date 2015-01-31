using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using SimpleViewEngine.Properties;
using SimpleViewEngine.Serializer;

namespace SimpleViewEngine
{
    /// <summary>
    /// Represents a simple HTML view engine for ASP .NET MVC applications.
    /// </summary>
    public class HtmlViewEngine : VirtualPathProviderViewEngine
    {
        private static string serverTagPrefix = "srv";

        private readonly DateTime? m_cacheExpiration;
        private IModelSerializer m_serializer;

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

            m_serializer = new DefaultModelSerializer();

            ViewLocationFormats = new[] { "~/views/{1}/{0}.html", "~/views/shared/{0}.html" };
            PartialViewLocationFormats = new[] { "~/views/{1}/{0}.partial.html", "~/views/shared/{0}.partial.html" };
            FileExtensions = new[] { "html" };
        }

        /// <summary>
        /// Gets or sets the server tag prefix. The default value is "srv".
        /// </summary>
        /// <exception cref="ArgumentNullException">If the value is null.</exception>
        /// <exception cref="ArgumentException">
        /// If the value contains any other characters besides lowercase letters and dashes.
        /// It cannot start or end with a dash.
        /// </exception>
        public static string ServerTagPrefix
        {
            get
            {
                return serverTagPrefix;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (!Regex.IsMatch(value, @"^[a-z\-]+$"))
                {
                    throw new ArgumentException(Resources.InvalidServerTagName, "value");
                }

                if (value.StartsWith("-") || value.EndsWith("-"))
                {
                    throw new ArgumentException(Resources.InvalidServerTagName, "value");
                }

                serverTagPrefix = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view engine supports view models passed
        /// to the view from the controller. The default value is false.
        /// </summary>
        public bool AntiForgeryTokenSupport { get; set; }

        /// <summary>
        /// Gets or sets the application version. It can be appended to CSS and JavaScript
        /// links using the <code>:version</code> URL variable.
        /// </summary>
        public string AppVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the view engine supports MVC content bundles.
        /// The default value is false.
        /// </summary>
        public bool BundleSupport { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the output HTML should be minified.
        /// The default value is false.
        /// </summary>
        public bool MinifyHtml { get; set; }

        /// <summary>
        /// Gets or sets a value for the model variable name that gets returned from the controller.
        /// Set this property to "null" (default) to disable JavaScript model support. The field
        /// name must support JavaScript naming conventions (no extended characters) or it will be
        /// ignored.
        /// </summary>
        public string ModelPropertyName { get; set; }

        /// <summary>
        /// Gets or sets the model JSON serializer.
        /// </summary>
        public IModelSerializer ModelSerializer
        {
            get
            {
                return m_serializer;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                m_serializer = value;
            }
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

            return new HtmlView(m_serializer, filePath, AppVersion, AntiForgeryTokenSupport, BundleSupport, null,
                                ModelPropertyName, MinifyHtml);
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

            return new HtmlView(m_serializer, filePath, AppVersion, AntiForgeryTokenSupport, BundleSupport,
                                m_cacheExpiration, ModelPropertyName, MinifyHtml);
        }
    }
}
