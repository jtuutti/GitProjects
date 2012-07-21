using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Routing;
using RestFoundation.Runtime;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation
{
    /// <summary>
    /// Represents a route builder.
    /// </summary>
    public sealed class RouteBuilder
    {
        private const char Slash = '/';
        private const char Tilda = '~';

        private static readonly Type urlAttributeType = typeof(UrlAttribute);
        private readonly RouteCollection m_routes;
        private readonly IHttpMethodResolver m_httpMethodResolver;
        private readonly IBrowserDetector m_browserDetector;

        internal RouteBuilder(RouteCollection routes, IHttpMethodResolver httpMethodResolver, IBrowserDetector browserDetector)
        {
            if (routes == null) throw new ArgumentNullException("routes");
            if (httpMethodResolver == null) throw new ArgumentNullException("httpMethodResolver");
            if (browserDetector == null) throw new ArgumentNullException("browserDetector");

            m_routes = routes;
            m_httpMethodResolver = httpMethodResolver;
            m_browserDetector = browserDetector;
        }

        /// <summary>
        /// Maps a REST service to the provided relative URL.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="url">The relative URL.</param>
        /// <returns>The route builder.</returns>
        public RouteConfiguration MapRestRoute<TContract>(string url)
        {
            return MapRestRoute(url, typeof(TContract), false);
        }

        /// <summary>
        /// Maps a REST service to the provided relative URL.
        /// </summary>
        /// <param name="url">The relative URL.</param>
        /// <param name="contractType">The service contract type.</param>
        /// <returns>The route builder.</returns>
        public RouteConfiguration MapRestRoute(string url, Type contractType)
        {
            return MapRestRoute(url, contractType, false);
        }

        /// <summary>
        /// Maps a REST service to the provided relative URL. An asynchrounous
        /// REST handler will be used. It is only recommended to map asynchronous
        /// routes for services that perform heavy operations that can block
        /// the worker process or run out of the ASP .NET thread pool.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="url">The relative URL.</param>
        /// <returns>The route builder.</returns>
        public RouteConfiguration MapRestRouteAsync<TContract>(string url)
        {
            return MapRestRoute(url, typeof(TContract), true);
        }

        /// <summary>
        /// Maps a REST service to the provided relative URL. An asynchrounous
        /// REST handler will be used. It is only recommended to map asynchronous
        /// routes for services that perform heavy operations that can block
        /// the worker process or run out of the ASP .NET thread pool.
        /// </summary>
        /// <param name="url">The relative URL.</param>
        /// <param name="contractType">The service contract type.</param>
        /// <returns>The route builder.</returns>
        public RouteConfiguration MapRestRouteAsync(string url, Type contractType)
        {
            return MapRestRoute(url, contractType, true);
        }

        /// <summary>
        /// Maps a web forms .ASPX page to a route.
        /// </summary>
        /// <param name="url">The relative URL.</param>
        /// <param name="relativePagePath">A relative path to the .ASPX page.</param>
        /// <returns></returns>
        public RouteBuilder MapPageRoute(string url, string relativePagePath)
        {
            if (String.IsNullOrEmpty(url)) throw new ArgumentNullException("url");
            if (String.IsNullOrEmpty(relativePagePath)) throw new ArgumentNullException("relativePagePath");

            m_routes.MapPageRoute(null, url, relativePagePath);
            return this;
        }

        private static HashSet<HttpMethod> AddHttpMethods(IEnumerable<HttpMethod> urlMethods)
        {
            var allowedMethods = new HashSet<HttpMethod>();

            foreach (var urlMethod in urlMethods)
            {
                allowedMethods.Add(urlMethod);
            }

            return allowedMethods;
        }

        private static HashSet<HttpMethod> UpdateHttpMethods(HashSet<HttpMethod> allowedMethods, IEnumerable<HttpMethod> urlMethods)
        {
            foreach (var urlMethod in urlMethods)
            {
                allowedMethods.Add(urlMethod);
            }

            return allowedMethods;
        }

        private static string ConcatUrl(string url, string urlTemplate)
        {
            return String.Concat(url.TrimEnd(Slash), Slash, urlTemplate.TrimStart(Slash, Tilda));
        }

        private RouteConfiguration MapRestRoute(string url, Type contractType, bool isAsync)
        {
            if (url == null) throw new ArgumentNullException("url");
            if (url.Trim().Length == 0) throw new ArgumentException("Route url cannot be null or empty", "url");
            if (contractType == null) throw new ArgumentNullException("contractType");
            if (!contractType.IsInterface) throw new ArgumentException("Service contract type must be an interface", "contractType");

            var serviceMethods = contractType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                                    .Where(m => m.GetCustomAttributes(urlAttributeType, false).Length > 0);

            List<ServiceMethodMetadata> methodMetadata = GenerateMethodMetadata(contractType, serviceMethods, url);
            ServiceMethodRegistry.ServiceMethods.AddOrUpdate(new ServiceMetadata(contractType, url), t => methodMetadata, (t, u) => methodMetadata);

            IEnumerable<IRestHandler> routeHandlers = MapRoutes(methodMetadata, url, contractType, isAsync);
            return new RouteConfiguration(routeHandlers);
        }

        private List<ServiceMethodMetadata> GenerateMethodMetadata(Type serviceContractType, IEnumerable<MethodInfo> methods, string url)
        {
            var urlAttributes = new List<ServiceMethodMetadata>();

            foreach (MethodInfo method in methods)
            {
                foreach (UrlAttribute urlAttribute in Attribute.GetCustomAttributes(method, urlAttributeType, false).Cast<UrlAttribute>())
                {
                    var methodMetadata = new ServiceMethodMetadata(url, method, urlAttribute);
                    var httpMethods = urlAttribute.HttpMethods ?? (urlAttribute.HttpMethods = m_httpMethodResolver.Resolve(method));

                    urlAttributes.Add(methodMetadata);

                    HttpMethodRegistry.HttpMethods.AddOrUpdate(new RouteMetadata(serviceContractType.AssemblyQualifiedName, urlAttribute.UrlTemplate),
                                                               template => AddHttpMethods(httpMethods),
                                                               (template, allowedMethods) => UpdateHttpMethods(allowedMethods, httpMethods));
                }
            }

            return urlAttributes;
        }

        private IEnumerable<IRestHandler> MapRoutes(IEnumerable<ServiceMethodMetadata> methodMetadata, string url, Type serviceContractType, bool isAsync)
        {
            var routeHandlers = new List<IRestHandler>();
            var orderedMethodMetadata = methodMetadata.OrderByDescending(m => m.UrlInfo.Priority);

            url = url.Trim();

            foreach (ServiceMethodMetadata metadata in orderedMethodMetadata)
            {
                var defaults = new RouteValueDictionary
                                   {
                                       { RouteConstants.ServiceContractType, serviceContractType.AssemblyQualifiedName },
                                       { RouteConstants.ServiceUrl, url },
                                       { RouteConstants.UrlTemplate, metadata.UrlInfo.UrlTemplate.Trim() }
                                   };

                var constraints = new RouteValueDictionary
                {
                    { RouteConstants.RouteConstraint, new ServiceRouteConstraint(metadata) }
                };

                var routeHandler = isAsync ? (IRestHandler) Rest.Active.ServiceLocator.GetService<RestAsyncHandler>() : Rest.Active.ServiceLocator.GetService<RestHandler>();
                routeHandlers.Add(routeHandler);

                string serviceUrl = ConcatUrl(url, metadata.UrlInfo.UrlTemplate.Trim());

                if (!String.IsNullOrWhiteSpace(metadata.UrlInfo.WebPageUrl))
                {
                    SetBrowserRoutes(serviceUrl, metadata);
                }

                m_routes.Add(new Route(serviceUrl, defaults, constraints, routeHandler));
            }

            return routeHandlers;
        }

        private void SetBrowserRoutes(string serviceUrl, ServiceMethodMetadata metadata)
        {
            string externalUrl = metadata.UrlInfo.WebPageUrl.Trim();

            var constraints = new RouteValueDictionary
            {
                { RouteConstants.BrowserConstraint, new BrowserConstraint(m_browserDetector) }
            };

            if (externalUrl.StartsWith("~/", StringComparison.Ordinal) && externalUrl.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
            {
                m_routes.MapPageRoute(null, serviceUrl, externalUrl, false, new RouteValueDictionary(), constraints);
                return;
            }

            var defaults = new RouteValueDictionary
            {
                { RouteConstants.WebPageUrl, externalUrl }
            };

            m_routes.Add(new Route(serviceUrl, defaults, constraints, new BrowserRedirectHandler()));
        }
    }
}
