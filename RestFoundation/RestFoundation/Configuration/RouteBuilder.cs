// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Routing;
using RestFoundation.Resources;
using RestFoundation.Runtime;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation.Configuration
{
    /// <summary>
    /// Represents a route builder for the specified relative URL.
    /// </summary>
    public sealed class RouteBuilder
    {
        private const char Slash = '/';
        private const char Tilda = '~';

        private static readonly Type urlAttributeType = typeof(UrlAttribute);
        private static readonly Regex serviceNameRegex = new Regex(@"^[_a-zA-Z0-9\-]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex urlParameterRegex = new Regex(@"\{([_a-zA-Z0-9]+)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly object addSyncRoot = new Object();
        private static readonly object updateSyncRoot = new Object();

        private readonly string m_relativeUrl;
        private readonly RouteCollection m_routes;
        private TimeSpan? m_asyncTimeout;

        internal RouteBuilder(string relativeUrl, RouteCollection routes)
        {
            if (String.IsNullOrEmpty(relativeUrl))
            {
                throw new ArgumentNullException("relativeUrl");
            }

            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }

            m_relativeUrl = relativeUrl.Trim();
            m_routes = routes;
        }

        /// <summary>
        /// Sets a timeout for asynchronous tasks returned by the services for the mapped URL. The default value is
        /// <see cref="TimeSpan.Zero"/> representing no timeout.
        /// </summary>
        /// <param name="timeout">The timeout value.</param>
        /// <returns>The route builder.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If a negative, non-infinite timeout is provided.</exception>
        public RouteBuilder WithAsyncTimeout(TimeSpan timeout)
        {
            if (timeout.TotalSeconds < -1)
            {
                throw new ArgumentOutOfRangeException("timeout", Global.InvalidAsyncTimeout);
            }

            m_asyncTimeout = timeout.TotalSeconds > 0 ? timeout : TimeSpan.Zero;
            return this;
        }

        /// <summary>
        /// Maps the relative URL to a service contract.
        /// </summary>
        /// <param name="contractType">The service contract type./</param>
        /// <returns>The route configuration.</returns>
        /// <exception cref="ArgumentException">
        /// If the service contract type is not an interface or a concrete class that defines its own contract.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If the relative URL is invalid or has already been mapped.
        /// </exception>
        public RouteConfiguration ToServiceContract(Type contractType)
        {
            if (contractType == null)
            {
                throw new ArgumentNullException("contractType");
            }

            return MapUrl(contractType);
        }

        /// <summary>
        /// Maps the relative URL to a service contract.
        /// </summary>
        /// <typeparam name="T">The service contract type.</typeparam>
        /// <returns>The route configuration.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the relative URL is invalid or has already been mapped.
        /// </exception>
        public RouteConfiguration ToServiceContract<T>()
        {
            return MapUrl(typeof(T));
        }

        /// <summary>
        /// Maps the relative URL to an HTTP handler of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The HTTP handler type.</typeparam>
        /// <exception cref="InvalidOperationException">
        /// If the HTTP handler type is not a concrete class.
        /// </exception>
        public void ToHttpHandler<T>()
            where T : HttpHandler
        {
            ToHttpHandler<T>(null, null);
        }

        /// <summary>
        /// Maps the relative URL to an HTTP handler of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The HTTP handler type.</typeparam>
        /// <param name="defaults">
        /// The values to use for route parameters if they are missing in the URL.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If the HTTP handler type is not a concrete class.
        /// </exception>
        public void ToHttpHandler<T>(RouteHash defaults)
            where T : HttpHandler
        {
            ToHttpHandler<T>(defaults, null);
        }

        /// <summary>
        /// Maps the relative URL to an HTTP handler of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The HTTP handler type.</typeparam>
        /// <param name="defaults">
        /// The values to use for route parameters if they are missing in the URL.
        /// </param>
        /// <param name="constraints">
        /// The contraints to use for the route and its parameters.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If the HTTP handler type is not a concrete class.
        /// </exception>
        public void ToHttpHandler<T>(RouteHash defaults, RouteHash constraints)
            where T : HttpHandler
        {
            Type handlerType = typeof(T);

            if (!handlerType.IsClass || handlerType.IsAbstract)
            {
                throw new InvalidOperationException(Global.AbstractHttpHandler);
            }

            var handler = Rest.Configuration.ServiceLocator.GetService<T>();
            RouteValueDictionary methodConstraints = GetHttpHandlerRouteConstraints(handler.AllowedMethods);

            if (methodConstraints != null)
            {
                if (constraints == null)
                {
                    constraints = new RouteHash();
                }

                MergeHttpHandlerConstraints(constraints, methodConstraints);
            }

            m_routes.Add(null, new Route(m_relativeUrl, defaults, constraints, handler));
        }

        /// <summary>
        /// Maps the relative URL to an HTML page.
        /// </summary>
        /// <param name="virtualPageUrl">A virtual URL to an HTML file.</param>
        public void ToHtmlPage(string virtualPageUrl)
        {
            ToHtmlPage(virtualPageUrl, null);
        }

        /// <summary>
        /// Maps the relative URL to an HTML page.
        /// </summary>
        /// <param name="virtualPageUrl">A virtual URL to an HTML file.</param>
        /// <param name="defaults">
        /// The values to use for route parameters if they are missing in the URL.
        /// </param>
        public void ToHtmlPage(string virtualPageUrl, RouteHash defaults)
        {
            if (virtualPageUrl == null)
            {
                throw new ArgumentNullException("virtualPageUrl");
            }

            m_routes.Add(null, new Route(m_relativeUrl, defaults, new HtmlRouteHandler(virtualPageUrl)));
        }

        /// <summary>
        /// Maps the relative URL to a web forms page.
        /// </summary>
        /// <param name="virtualPageUrl">A virtual URL to an ASPX file.</param>
        public void ToWebFormsPage(string virtualPageUrl)
        {
            ToWebFormsPage(virtualPageUrl, null);
        }

        /// <summary>
        /// Maps the relative URL to a web forms page.
        /// </summary>
        /// <param name="virtualPageUrl">A virtual URL to an ASPX file.</param>
        /// <param name="defaults">
        /// The values to use for route parameters if they are missing in the URL.
        /// </param>
        public void ToWebFormsPage(string virtualPageUrl, RouteHash defaults)
        {
            if (virtualPageUrl == null)
            {
                throw new ArgumentNullException("virtualPageUrl");
            }

            if (defaults != null)
            {
                m_routes.MapPageRoute(null, m_relativeUrl, virtualPageUrl, false, defaults);
            }
            else
            {
                m_routes.MapPageRoute(null, m_relativeUrl, virtualPageUrl, false);
            }
        }

        private static HashSet<HttpMethod> AddHttpMethods(IEnumerable<HttpMethod> urlMethods)
        {
            lock (addSyncRoot)
            {
                var allowedMethods = new HashSet<HttpMethod>();

                foreach (var urlMethod in urlMethods)
                {
                    allowedMethods.Add(urlMethod);
                }

                return allowedMethods;
            }
        }

        private static HashSet<HttpMethod> UpdateHttpMethods(HashSet<HttpMethod> allowedMethods, IEnumerable<HttpMethod> urlMethods)
        {
            lock (updateSyncRoot)
            {
                foreach (var urlMethod in urlMethods)
                {
                    allowedMethods.Add(urlMethod);
                }

                return allowedMethods;
            }
        }

        private static string ConcatUrl(string url, string urlTemplate)
        {
            return String.Concat(url.TrimEnd(Slash), Slash, urlTemplate.TrimStart(Slash, Tilda));
        }

        private static List<ServiceMethodMetadata> GenerateMethodMetadata(Type serviceContractType, IEnumerable<MethodInfo> methods, string url)
        {
            var urlAttributes = new List<ServiceMethodMetadata>();
            var httpMethodResolver = Rest.Configuration.ServiceLocator.GetService<IHttpMethodResolver>();

            var metadataCollection = from m in methods
                                     from a in m.GetCustomAttributes<UrlAttribute>(false)
                                     select new { Method = m, Attribute = a };

            foreach (var metadata in metadataCollection)
            {
                var methodMetadata = new ServiceMethodMetadata(url, metadata.Method, metadata.Attribute);
                var httpMethods = metadata.Attribute.HttpMethods ?? (metadata.Attribute.HttpMethods = httpMethodResolver.Resolve(metadata.Method));

                if (metadata.Attribute.Priority == 0 && metadata.Method.GetParameters().Any(x => x.DefaultValue != DBNull.Value))
                {
                    metadata.Attribute.Priority = -1;
                }

                urlAttributes.Add(methodMetadata);

                HttpMethodRegistry.HttpMethods.AddOrUpdate(new RouteMetadata(serviceContractType.AssemblyQualifiedName, metadata.Attribute.UrlTemplate),
                                                           template => AddHttpMethods(httpMethods),
                                                           (template, allowedMethods) => UpdateHttpMethods(allowedMethods, httpMethods));
            }

            return urlAttributes;
        }

        private static void ConfigureOptionalRouteParameters(ServiceMethodMetadata metadata, RouteValueDictionary defaults)
        {
            var parameters = urlParameterRegex.Matches(metadata.UrlInfo.UrlTemplate);

            for (int i = 0; i < parameters.Count; i++)
            {
                ConfigureOptionalParameter(metadata, parameters[i].Groups[1].Value, defaults);
            }
        }

        private static void ConfigureOptionalParameter(ServiceMethodMetadata metadata, string parameterValue, RouteValueDictionary defaults)
        {
            ParameterInfo methodParameter = metadata.MethodInfo.GetParameters().FirstOrDefault(p => p.Name == parameterValue);

            if (methodParameter == null || methodParameter.DefaultValue == DBNull.Value)
            {
                return;
            }

            if (methodParameter.DefaultValue == null)
            {
                defaults[methodParameter.Name] = UrlParameter.Optional;
            }
            else
            {
                defaults[methodParameter.Name] = methodParameter.DefaultValue;
            }
        }

        private static RouteValueDictionary GetRouteConstraints(ServiceMethodMetadata metadata)
        {
            return new RouteValueDictionary
            {
                { ServiceCallConstants.RouteConstraint, new ServiceRouteConstraint(metadata) }
            };
        }

        private static RouteValueDictionary GetHttpHandlerRouteConstraints(IReadOnlyCollection<HttpMethod> allowedHttpMethods)
        {
            if (allowedHttpMethods == null || !allowedHttpMethods.Any())
            {
                return null;
            }

            return new RouteValueDictionary
            {
                { ServiceCallConstants.RouteConstraint, new HttpHandlerRouteConstraint(allowedHttpMethods) }
            };
        }

        private static RouteValueDictionary GetBrowserRouteConstraints()
        {
            return new RouteValueDictionary
            {
                {
                    ServiceCallConstants.BrowserConstraint, new BrowserRouteConstraint(Rest.Configuration.ServiceLocator.GetService<IContentNegotiator>(),
                                                                                       Rest.Configuration.ServiceLocator.GetService<IHttpRequest>())
                }
            };
        }

        private static void MergeHttpHandlerConstraints(RouteHash constraints, IEnumerable<KeyValuePair<string, object>> methodConstraints)
        {
            foreach (KeyValuePair<string, object> constraint in methodConstraints)
            {
                constraints[constraint.Key] = constraint.Value;
            }
        }

        private RouteValueDictionary GetRouteDefaults(ServiceMethodMetadata metadata, Type serviceContractType)
        {
            var defaults = new RouteValueDictionary
            {
                { ServiceCallConstants.ServiceContractType, serviceContractType.AssemblyQualifiedName },
                { ServiceCallConstants.ServiceUrl, m_relativeUrl },
                { ServiceCallConstants.UrlTemplate, metadata.UrlInfo.UrlTemplate.Trim() }
            };

            return defaults;
        }

        private void AddServiceMethod(Type contractType, List<ServiceMethodMetadata> metadata)
        {
            if (ServiceMethodRegistry.ServiceMethods.Any(m => String.Equals(m_relativeUrl, m.Key.Url, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Global.AlreadyMapped, m_relativeUrl));
            }

            ServiceMethodRegistry.ServiceMethods.AddOrUpdate(new ServiceMetadata(contractType, m_relativeUrl), t => metadata, (t, u) => metadata);
        }

        private RouteConfiguration MapUrl(Type contractType)
        {
            if (contractType == null)
            {
                throw new ArgumentNullException("contractType");
            }

            if (!contractType.IsInterface && !contractType.IsClass)
            {
                throw new ArgumentException(Global.InvalidServiceContract, "contractType");
            }

            if (contractType.IsClass && (contractType.IsAbstract || !Attribute.IsDefined(contractType, typeof(ServiceContractAttribute), true)))
            {
                throw new ArgumentException(Global.InvalidServiceImplementation, "contractType");
            }

            if (!serviceNameRegex.IsMatch(m_relativeUrl))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Global.InvalidServiceName, m_relativeUrl));
            }

            if (ServiceContractTypeRegistry.GetType(contractType.AssemblyQualifiedName) == null)
            {
                throw new ArgumentException(Global.InvalidServiceContract, "contractType");
            }

            var serviceMethods = contractType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                             .Where(m => Attribute.IsDefined(m, urlAttributeType, false));

            List<ServiceMethodMetadata> methodMetadata = GenerateMethodMetadata(contractType, serviceMethods, m_relativeUrl);
            AddServiceMethod(contractType, methodMetadata);

            IEnumerable<IRestServiceHandler> routeHandlers = MapRoutes(contractType, methodMetadata);
            return new RouteConfiguration(routeHandlers);
        }

        private IEnumerable<IRestServiceHandler> MapRoutes(Type serviceContractType, IEnumerable<ServiceMethodMetadata> metadata)
        {
            var routeHandlers = new List<IRestServiceHandler>();
            var orderedMethodMetadata = metadata.OrderByDescending(m => m.UrlInfo.Priority);

            foreach (ServiceMethodMetadata orderMetadata in orderedMethodMetadata)
            {
                MapRoute(serviceContractType, orderMetadata, routeHandlers);
            }

            return routeHandlers;
        }

        private void MapRoute(Type serviceContractType, ServiceMethodMetadata metadata, List<IRestServiceHandler> routeHandlers)
        {
            RouteValueDictionary defaults = GetRouteDefaults(metadata, serviceContractType);
            RouteValueDictionary constraints = GetRouteConstraints(metadata);

            ConfigureOptionalRouteParameters(metadata, defaults);

            var routeHandler = Rest.Configuration.ServiceLocator.GetService<IRestServiceHandler>();

            if (m_asyncTimeout.HasValue)
            {
                routeHandler.ServiceAsyncTimeout = m_asyncTimeout.Value;
            }

            routeHandlers.Add(routeHandler);

            string serviceUrl = ConcatUrl(m_relativeUrl, metadata.UrlInfo.UrlTemplate.Trim());

            if (!String.IsNullOrWhiteSpace(metadata.UrlInfo.WebPageUrl))
            {
                SetBrowserRoutes(serviceUrl, metadata);
            }

            string routeName = RouteNameHelper.GetRouteName(metadata.ServiceUrl, metadata.MethodInfo);

            m_routes.Add(routeName, new Route(serviceUrl, defaults, constraints, routeHandler));
        }

        private void SetBrowserRoutes(string serviceUrl, ServiceMethodMetadata metadata)
        {
            string externalUrl = metadata.UrlInfo.WebPageUrl.Trim();

            RouteValueDictionary constraints = GetBrowserRouteConstraints();

            if (externalUrl.StartsWith("~/", StringComparison.Ordinal) && externalUrl.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
            {
                m_routes.MapPageRoute(null, serviceUrl, externalUrl, false, new RouteValueDictionary(), constraints);
                return;
            }

            var defaults = new RouteValueDictionary
            {
                { ServiceCallConstants.WebPageUrl, externalUrl }
            };

            m_routes.Add(new Route(serviceUrl, defaults, constraints, new BrowserRedirectHandler()));
        }
    }
}
