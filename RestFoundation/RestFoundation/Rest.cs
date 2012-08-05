// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Web.Routing;
using System.Web.Util;
using RestFoundation.Formatters;
using RestFoundation.Runtime;
using RestFoundation.Runtime.Handlers;
using RestFoundation.ServiceLocation;

namespace RestFoundation
{
    /// <summary>
    /// Represents the REST Framework configuration class.
    /// </summary>
    public sealed class Rest
    {
        private static readonly object syncRoot = new object();

        private static readonly ICollection<Type> serviceContextTypes = new ReadOnlyCollection<Type>(new[]
        {
            typeof(IServiceContext),
            typeof(IHttpRequest),
            typeof(IHttpResponse),
            typeof(IHttpResponseOutput),
            typeof(IServiceCache)
        });

        /// <summary>
        /// Gets the active REST Foundation configuration.
        /// </summary>
        public static Rest Active { get; private set; }

        /// <summary>
        /// Gets the REST Foundation assembly instance for IoC support.
        /// </summary>
        public static Assembly FoundationAssembly
        {
            get
            {
                return typeof(Rest).Assembly;
            }
        }

        /// <summary>
        /// Gets the service context dependent types.
        /// </summary>
        public static ICollection<Type> ServiceContextTypes
        {
            get
            {
                return serviceContextTypes;
            }
        }

        /// <summary>
        /// Gets the service locator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; private set; }

        /// <summary>
        /// Gets the JQuery URL used by the service help and proxy interface.
        /// </summary>
        public string JQueryUrl { get; internal set; }

        internal bool IsServiceProxyInitialized { get; set; }
        internal string ServiceProxyRelativeUrl { get; set; }
        internal string DefaultMediaType { get; private set; }
        internal IDictionary<string, string> ResponseHeaders { get; private set; }
        internal string IndexPageRelativeUrl { get; private set; }

        /// <summary>
        /// Configures the REST Foundation with the provided <see cref="IServiceLocator"/>.
        /// </summary>
        /// <param name="resolver">The dependency resolver.</param>
        /// <returns>The configuration object.</returns>
        /// <exception cref="InvalidOperationException">If the REST foundation has already been configured.</exception>
        public static Rest Configure(IServiceLocator resolver)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException("resolver");
            }

            if (Active != null)
            {
                throw new InvalidOperationException("REST Foundation has already been configured.");
            }

            lock (syncRoot)
            {
                if (Active != null)
                {
                    throw new InvalidOperationException("REST Foundation has already been configured.");
                }

                RouteCollection routes = RouteTable.Routes;
                routes.Add(new Route(String.Empty, resolver.GetService<RootRouteHandler>()));

                RequestValidator.Current = new ServiceRequestValidator();

                Active = new Rest
                {
                    ServiceLocator = resolver
                };

                return Active;
            }
        }

        /// <summary>
        /// Calls the provided service proxy configuration object to set up service help and proxy UI for the services.
        /// </summary>
        /// <param name="configuration">The service help and proxy configuration.</param>
        /// <returns>The configuration object.</returns>
        public Rest ConfigureServiceHelpAndProxy(Action<ServiceProxyConfiguration> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            configuration(new ServiceProxyConfiguration());
            return this;
        }

        /// <summary>
        /// Adds media type formatters for the JSONP support.
        /// </summary>
        /// <returns>The configuration object.</returns>
        public Rest EnableJsonPSupport()
        {
            MediaTypeFormatterRegistry.SetFormatter("application/javascript", new JsonPFormatter());
            MediaTypeFormatterRegistry.SetFormatter("text/javascript", new JsonPFormatter());

            return this;
        }

        /// <summary>
        /// Sets the default media type in case the Accept HTTP header is not provided.
        /// </summary>
        /// <param name="mediaType">The media type.</param>
        /// <returns>The configuration object.</returns>
        public Rest WithDefaultMediaType(string mediaType)
        {
            if (String.IsNullOrEmpty(mediaType))
            {
                throw new ArgumentNullException("mediaType");
            }

            DefaultMediaType = mediaType;
            return this;
        }

        /// <summary>
        /// Calls the provided global behavior builder delegate to set or remove behaviors global to all services.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The configuration object.</returns>
        public Rest WithGlobalBehaviors(Action<GlobalBehaviorBuilder> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            builder(new GlobalBehaviorBuilder());
            return this;
        }

        /// <summary>
        /// Sets the exception handler global to all services.
        /// </summary>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <returns>The configuration object.</returns>
        public Rest WithGlobalExceptionHandler(IServiceExceptionHandler exceptionHandler)
        {
            if (exceptionHandler == null)
            {
                throw new ArgumentNullException("exceptionHandler");
            }

            ServiceExceptionHandlerRegistry.SetGlobalExceptionHandler(exceptionHandler);
            return this;
        }

        /// <summary>
        /// Sets the default page file name. The file must be in the root folder and only the file name must be
        /// provided.
        /// </summary>
        /// <param name="filename">The file name.</param>
        /// <returns>The configuration object.</returns>
        /// <exception cref="ArgumentException">
        /// If the file has an unsupported extension or a file path had been provided in addition to the name.
        /// </exception>
        public Rest WithIndexFileName(string filename)
        {
            if (String.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename");
            }

            if (filename.IndexOf('~') >= 0 || filename.IndexOf('/') >= 0 || filename.IndexOf('\\') >= 0 || filename.IndexOf(':') >= 0)
            {
                throw new ArgumentException("Only a file name can be specified. Relative or absolute paths/URLs are not supported.");
            }

            string extension = Path.GetExtension(filename);

            if (!String.Equals(extension, ".htm", StringComparison.OrdinalIgnoreCase) &&
                !String.Equals(extension, ".html", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Index files can only have .html or .htm exceptions.");
            }

            IndexPageRelativeUrl = "~/" + filename.Trim();
            return this;
        }

        /// <summary>
        /// Calls the provided media type formatter builder delegate to set or remove formatters.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The configuration object.</returns>
        public Rest WithMediaTypeFormatters(Action<MediaTypeFormatterBuilder> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            builder(new MediaTypeFormatterBuilder());
            return this;
        }

        /// <summary>
        /// Adds the provided header to all HTTP responses.
        /// </summary>
        /// <param name="headerName">The header name.</param>
        /// <param name="headerValue">The header value.</param>
        /// <returns>The configuration object.</returns>
        public Rest WithResponseHeader(string headerName, string headerValue)
        {
            if (String.IsNullOrEmpty(headerName))
            {
                throw new ArgumentNullException("headerName");
            }

            if (headerValue == null)
            {
                throw new ArgumentNullException("headerValue");
            }

            if (ResponseHeaders == null)
            {
                ResponseHeaders = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { headerName, headerValue }
                };
            }

            ResponseHeaders[headerName] = headerValue;
            return this;
        }

        /// <summary>
        /// Adds the provided response headers to all HTTP responses.
        /// </summary>
        /// <param name="responseHeaders">A dictionary of header names and values.</param>
        /// <returns>The configuration object.</returns>
        public Rest WithResponseHeaders(IDictionary<string, string> responseHeaders)
        {
            if (responseHeaders == null)
            {
                throw new ArgumentNullException("responseHeaders");
            }

            if (ResponseHeaders != null)
            {
                foreach (KeyValuePair<string, string> header in responseHeaders)
                {
                    ResponseHeaders[header.Key] = header.Value;
                }
            }
            else
            {
                ResponseHeaders = new SortedDictionary<string, string>(responseHeaders, StringComparer.OrdinalIgnoreCase);
            }

            return this;
        }

        /// <summary>
        /// Calls the provided type binder builder delegate to set or remove type binders.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The configuration object.</returns>
        public Rest WithTypeBinders(Action<TypeBinderBuilder> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            builder(new TypeBinderBuilder());
            return this;
        }

        /// <summary>
        /// Calls the provided URL builder delegate to set up URL routes to services and web form pages.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The configuration object.</returns>
        public Rest WithUrls(Action<UrlBuilder> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            builder(new UrlBuilder(RouteTable.Routes));
            return this;
        }
    }
}
