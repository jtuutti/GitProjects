using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Routing;
using RestFoundation.Runtime;

namespace RestFoundation
{
    public sealed class RouteBuilder
    {
        private const char Slash = '/';
        private const char Tilda = '~';

        private static readonly Type urlAttributeType = typeof(UrlAttribute);
        private readonly RouteCollection m_routes;
        private readonly IHttpMethodResolver m_httpMethodResolver;

        internal RouteBuilder(RouteCollection routes, IHttpMethodResolver httpMethodResolver)
        {
            if (routes == null) throw new ArgumentNullException("routes");
            if (httpMethodResolver == null) throw new ArgumentNullException("httpMethodResolver");

            m_routes = routes;
            m_httpMethodResolver = httpMethodResolver;
        }

        public RouteConfiguration MapRestRoute<TContract>(string url)
        {
            return MapRestRoute(url, typeof(TContract), false);
        }

        public RouteConfiguration MapRestRoute(string url, Type serviceContractType)
        {
            return MapRestRoute(url, serviceContractType, false);
        }

        public RouteConfiguration MapRestRouteAsync<TContract>(string url)
        {
            return MapRestRoute(url, typeof(TContract), true);
        }

        public RouteConfiguration MapRestRouteAsync(string url, Type serviceContractType)
        {
            return MapRestRoute(url, serviceContractType, true);
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

        private RouteConfiguration MapRestRoute(string url, Type serviceContractType, bool isAsync)
        {
            if (url == null) throw new ArgumentNullException("url");
            if (url.Trim().Length == 0) throw new ArgumentException("Route url cannot be null or empty", "url");
            if (serviceContractType == null) throw new ArgumentNullException("serviceContractType");
            if (!serviceContractType.IsInterface) throw new ArgumentException("Service contract type must be an interface", "serviceContractType");

            var serviceMethods = serviceContractType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                                    .Where(m => m.GetCustomAttributes(urlAttributeType, false).Length > 0);

            List<ServiceMethodMetadata> methodMetadata = GenerateMethodMetadata(serviceContractType, serviceMethods, url);
            ServiceMethodRegistry.ServiceMethods.AddOrUpdate(new ServiceMetadata(serviceContractType, url), t => methodMetadata, (t, u) => methodMetadata);

            IEnumerable<IRouteHandler> routeHandlers = MapRoutes(methodMetadata, url, serviceContractType, isAsync);
            return new RouteConfiguration(routeHandlers);
        }

        private List<ServiceMethodMetadata> GenerateMethodMetadata(Type serviceContractType, IEnumerable<MethodInfo> methods, string url)
        {
            var urlAttributes = new List<ServiceMethodMetadata>();

            foreach (MethodInfo method in methods)
            {
                var aclAttribute = (ValidateAclAttribute) (Attribute.GetCustomAttribute(method, typeof(ValidateAclAttribute), false) ??
                                                           Attribute.GetCustomAttribute(serviceContractType, typeof(ValidateAclAttribute), false));

                var outputCacheAttribute = (OutputCacheAttribute) Attribute.GetCustomAttribute(method, typeof(OutputCacheAttribute), false);

                foreach (UrlAttribute urlAttribute in Attribute.GetCustomAttributes(method, urlAttributeType, false).Cast<UrlAttribute>())
                {
                    if (urlAttribute.HttpMethods == null)
                    {
                        urlAttribute.HttpMethods = m_httpMethodResolver.Resolve(method);
                    }

                    var methodMetadata = new ServiceMethodMetadata(url, method, urlAttribute, aclAttribute, outputCacheAttribute);
                    urlAttributes.Add(methodMetadata);

                    var httpMethods = urlAttribute.HttpMethods;
                    HttpMethodRegistry.HttpMethods.AddOrUpdate(new RouteMetadata(serviceContractType.AssemblyQualifiedName, urlAttribute.UrlTemplate),
                                                               template => AddHttpMethods(httpMethods),
                                                               (template, allowedMethods) => UpdateHttpMethods(allowedMethods, httpMethods));
                }
            }

            return urlAttributes;
        }

        private IEnumerable<IRouteHandler> MapRoutes(IEnumerable<ServiceMethodMetadata> methodMetadata, string url, Type serviceContractType, bool isAsync)
        {
            var routeHandlers = new List<IRouteHandler>();
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

                var routeHandler = isAsync ? (IRouteHandler) Rest.Active.CreateObject<RestAsyncHandler>() : Rest.Active.CreateObject<RestHandler>();
                routeHandlers.Add(routeHandler);

                m_routes.Add(new Route(ConcatUrl(url, metadata.UrlInfo.UrlTemplate.Trim()), defaults, constraints, routeHandler));
            }

            return routeHandlers;
        }
    }
}
