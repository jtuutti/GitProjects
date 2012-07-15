using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Web.Routing;
using System.Web.Util;
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
        internal static readonly Rest Active = new Rest();

        private static readonly object syncRoot = new object();
        private static bool defaultUrlMapped;

        /// <summary>
        /// Gets the configuration object instance.
        /// </summary>
        public static Rest Configure
        {
            get
            {
                if (!(RequestValidator.Current is ServiceRequestValidator))
                {
                    RequestValidator.Current = new ServiceRequestValidator();
                }

                return Active;
            }
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
        /// Gets the object activator instance.
        /// </summary>
        public IObjectActivator Activator { get; private set; }

        /// <summary>
        /// Gets the JQuery URL used by the service help and proxy interface.
        /// </summary>
        public string JQueryUrl { get; internal set; }

        internal bool IsServiceProxyInitialized { get; set; }
        internal string ServiceProxyRelativeUrl { get; set; }
        internal IDictionary<string, string> ResponseHeaders { get; private set; }

        /// <summary>
        /// Sets an object activator for the configuration.
        /// </summary>
        /// <param name="activator">The object activator implementation.</param>
        /// <returns>The configuration object.</returns>
        public Rest WithObjectActivator(IObjectActivator activator)
        {
            if (activator == null) throw new ArgumentNullException("activator");

            if (Activator != null)
            {
                throw new InvalidOperationException("An object activator or an object factory has already been assigned.");
            }

            Activator = activator;

            MapDefaultUrl();
            return this;
        }

        /// <summary>
        /// Sets an object activator for the configuration that uses the provided factory delegate to create objects.
        /// </summary>
        /// <param name="factory">The object activator implementation.</param>
        /// <returns>The configuration object.</returns>
        public Rest WithObjectFactory(Func<Type, object> factory)
        {
            return WithObjectFactory(factory, obj => {});
        }

        /// <summary>
        /// Sets an object activator for the configuration that uses the provided factory delegate to create objects
        /// and the builder delegate to build up existing objects with property injection dependencies.
        /// </summary>
        /// <param name="factory">The object factory delegate.</param>
        /// <param name="builder">The object build up delegate.</param>
        /// <returns>The configuration object.</returns>
        public Rest WithObjectFactory(Func<Type, object> factory, Action<object> builder)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            if (builder == null) throw new ArgumentNullException("builder");

            if (Activator != null)
            {
                throw new InvalidOperationException("An object activator or an object factory has already been assigned.");
            }

            Activator = new DelegateObjectActivator(factory, builder);

            MapDefaultUrl();
            return this;
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

            builder(new RouteBuilder(RouteTable.Routes, Active.CreateObject<IHttpMethodResolver>(), Active.CreateObject<IBrowserDetector>()));
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

        internal object CreateObject(Type objectType)
        {
            if (objectType == null) throw new ArgumentNullException("objectType");

            if (Activator == null)
            {
                throw new ObjectActivationException("No object activator or factory has been assigned.");
            }

            try
            {
                return Activator.Create(objectType);
            }
            catch (Exception ex)
            {
                throw new ObjectActivationException(String.Format(CultureInfo.InvariantCulture, "Object of type '{0}' could not be activated.", objectType), ex);
            }
        }

        internal T CreateObject<T>()
        {
            Type objectType = typeof(T);

            try
            {
                return (T) CreateObject(objectType);
            }
            catch (Exception ex)
            {
                throw new ObjectActivationException(String.Format(CultureInfo.InvariantCulture, "Object of type '{0}' could not be activated.", objectType), ex);
            }
        }

        private static void MapDefaultUrl()
        {
            RouteCollection routes = RouteTable.Routes;

            if (routes == null)
            {
                throw new InvalidOperationException("No active routing table was found.");
            }

            if (defaultUrlMapped)
            {
                return;
            }

            lock (syncRoot)
            {
                if (!defaultUrlMapped)
                {
                    routes.Add(new Route(String.Empty, Active.CreateObject<RootRouteHandler>()));
                    defaultUrlMapped = true;
                }
            }
        }
    }
}
