using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Routing;
using System.Web.Util;
using RestFoundation.DependencyInjection;
using RestFoundation.Formatters;
using RestFoundation.Runtime;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation
{
    /// <summary>
    /// Represents the REST Framework configuration class.
    /// </summary>
    public sealed class Rest
    {
        private static readonly object syncRoot = new object();

        /// <summary>
        /// Gets the active REST Foundation configuration.
        /// </summary>
        public static Rest Active
        {
            get;
            private set;
        }

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
        /// Gets the dependency resolver.
        /// </summary>
        public IDependencyResolver DependencyResolver { get; private set; }

        /// <summary>
        /// Gets the JQuery URL used by the service help and proxy interface.
        /// </summary>
        public string JQueryUrl { get; internal set; }

        internal bool IsServiceProxyInitialized { get; set; }
        internal string ServiceProxyRelativeUrl { get; set; }
        internal IDictionary<string, string> ResponseHeaders { get; private set; }

        /// <summary>
        /// Configures the REST Foundation with the provided <see cref="IDependencyRegistry"/> object.
        /// </summary>
        /// <param name="resolver">The dependency resolver.</param>
        /// <returns>The configuration object.</returns>
        /// <exception cref="InvalidOperationException">If the REST foundation has already been configured.</exception>
        public static Rest Configure(IDependencyResolver resolver)
        {
            if (resolver == null) throw new ArgumentNullException("resolver");

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
                routes.Add(new Route(String.Empty, resolver.Resolve<RootRouteHandler>()));

                RequestValidator.Current = new ServiceRequestValidator();

                Active = new Rest
                {
                    DependencyResolver = resolver
                };

                return Active;
            }
        }

        /// <summary>
        /// Calls the provided global behavior builder delegate to set or remove behaviors global to all services.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The configuration object.</returns>
        public Rest WithGlobalBehaviors(Action<GlobalBehaviorBuilder> builder)
        {
            if (builder == null) throw new ArgumentNullException("builder");

            builder(new GlobalBehaviorBuilder());
            return this;
        }

        /// <summary>
        /// Calls the provided routing builder delegate to set up URL routes to services.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The configuration object.</returns>
        public Rest WithRoutes(Action<RouteBuilder> builder)
        {
            if (builder == null) throw new ArgumentNullException("builder");

            builder(new RouteBuilder(RouteTable.Routes, DependencyResolver.Resolve<IHttpMethodResolver>(), DependencyResolver.Resolve<IBrowserDetector>()));
            return this;
        }

        /// <summary>
        /// Calls the provided content formatter builder delegate to set or remove content formatters.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The configuration object.</returns>
        public Rest WithContentFormatters(Action<ContentFormatterBuilder> builder)
        {
            if (builder == null) throw new ArgumentNullException("builder");

            builder(new ContentFormatterBuilder());
            return this;
        }

        /// <summary>
        /// Calls the provided type binder builder delegate to set or remove type binders.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The configuration object.</returns>
        public Rest WithTypeBinders(Action<TypeBinderBuilder> builder)
        {
            if (builder == null) throw new ArgumentNullException("builder");

            builder(new TypeBinderBuilder());
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
            if (String.IsNullOrEmpty(headerName)) throw new ArgumentNullException("headerName");
            if (String.IsNullOrEmpty(headerValue)) throw new ArgumentNullException("headerValue");

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
            if (responseHeaders == null) throw new ArgumentNullException("responseHeaders");

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
        /// Adds content formatters for the JSONP support.
        /// </summary>
        /// <returns>The configuration object.</returns>
        public Rest EnableJsonPSupport()
        {
            ContentFormatterRegistry.SetFormatter("application/javascript", new JsonPFormatter());
            ContentFormatterRegistry.SetFormatter("text/javascript", new JsonPFormatter());

            return this;
        }

        /// <summary>
        /// Calls the provided service proxy configuration object to set up service help and proxy UI for the services.
        /// </summary>
        /// <param name="configuration">The service help and proxy configuration.</param>
        /// <returns>The configuration object.</returns>
        public Rest ConfigureServiceHelpAndProxy(Action<ServiceProxyConfiguration> configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            configuration(new ServiceProxyConfiguration());
            return this;
        }
    }
}
