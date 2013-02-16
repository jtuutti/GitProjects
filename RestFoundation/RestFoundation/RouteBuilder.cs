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
using RestFoundation.Runtime;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation
{
    /// <summary>
    /// Represents a route builder for the specified relative URL.
    /// </summary>
    public sealed class RouteBuilder
    {
        private const char Slash = '/';
        private const char Tilda = '~';

        private static readonly Type urlAttributeType = typeof(UrlAttribute);
        private static readonly object syncRoot = new object();
        private static readonly Regex urlParameterRegex = new Regex(@"\{([_a-zA-Z0-9]+)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly string m_relativeUrl;
        private readonly RouteCollection m_routes;
        private TimeSpan? m_serviceTimeout;

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
        /// Sets a service method execution timeout for the mapped URL. The default value is 60 seconds.
        /// </summary>
        /// <param name="timeout">The timeout value.</param>
        /// <returns>The route builder.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If a negative, non-infinite timeout is provided.</exception>
        public RouteBuilder WithServiceTimeout(TimeSpan timeout)
        {
            if (timeout.TotalSeconds < -1)
            {
                throw new ArgumentOutOfRangeException("timeout", RestResources.InvalidServiceMethodTimeout);
            }

            m_serviceTimeout = timeout.TotalSeconds > 0 ? timeout : TimeSpan.Zero;
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
        /// <exception cref="InvalidOperationException">If the relative URL has already been mapped.</exception>
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
        /// <exception cref="ArgumentException">
        /// If the service contract type is not an interface or a concrete class that defines its own contract.
        /// </exception>
        /// <exception cref="InvalidOperationException">If the relative URL has already been mapped.</exception>
        public RouteConfiguration ToServiceContract<T>()
        {
            return MapUrl(typeof(T));
        }

        /// <summary>
        /// Maps the relative URL to a web forms page.
        /// </summary>
        /// <param name="virtualPageUrl">A virtual URL to the ASPX file.</param>
        public void ToWebFormsPage(string virtualPageUrl)
        {
            if (virtualPageUrl == null)
            {
                throw new ArgumentNullException("virtualPageUrl");
            }

            m_routes.MapPageRoute(null, m_relativeUrl, virtualPageUrl);
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

        private static List<ServiceMethodMetadata> GenerateMethodMetadata(Type serviceContractType, IEnumerable<MethodInfo> methods, string url)
        {
            var urlAttributes = new List<ServiceMethodMetadata>();
            var httpMethodResolver = Rest.Configuration.ServiceLocator.GetService<IHttpMethodResolver>();

            foreach (MethodInfo method in methods)
            {
                foreach (UrlAttribute urlAttribute in Attribute.GetCustomAttributes(method, urlAttributeType, false).Cast<UrlAttribute>())
                {
                    var methodMetadata = new ServiceMethodMetadata(url, method, urlAttribute);
                    var httpMethods = urlAttribute.HttpMethods ?? (urlAttribute.HttpMethods = httpMethodResolver.Resolve(method));

                    if (urlAttribute.Priority == 0 && method.GetParameters().Any(x => x.DefaultValue != DBNull.Value))
                    {
                        urlAttribute.Priority = -1;
                    }

                    urlAttributes.Add(methodMetadata);

                    HttpMethodRegistry.HttpMethods.AddOrUpdate(new RouteMetadata(serviceContractType.AssemblyQualifiedName, urlAttribute.UrlTemplate),
                                                               template => AddHttpMethods(httpMethods),
                                                               (template, allowedMethods) => UpdateHttpMethods(allowedMethods, httpMethods));
                }
            }

            return urlAttributes;
        }

        private static void ConfigureOptionalRouteParameters(ServiceMethodMetadata metadata, RouteValueDictionary defaults)
        {
            var routeParameters = urlParameterRegex.Matches(metadata.UrlInfo.UrlTemplate);

            for (int i = 0; i < routeParameters.Count; i++)
            {
                ParameterInfo methodParameter = metadata.MethodInfo.GetParameters().FirstOrDefault(p => p.Name == routeParameters[i].Groups[1].Value);

                if (methodParameter == null || methodParameter.DefaultValue == DBNull.Value)
                {
                    continue;
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
        }

        private RouteConfiguration MapUrl(Type contractType)
        {
            if (contractType == null)
            {
                throw new ArgumentNullException("contractType");
            }

            if (!contractType.IsInterface && !contractType.IsClass)
            {
                throw new ArgumentException(RestResources.InvalidServiceContract, "contractType");
            }

            if (contractType.IsClass && (contractType.IsAbstract || !Attribute.IsDefined(contractType, typeof(ServiceContractAttribute), true)))
            {
                throw new ArgumentException(RestResources.InvalidServiceImplementation, "contractType");
            }

            var serviceMethods = contractType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                             .Where(m => Attribute.IsDefined(m, urlAttributeType, false));

            List<ServiceMethodMetadata> methodMetadata = GenerateMethodMetadata(contractType, serviceMethods, m_relativeUrl);

            lock (syncRoot)
            {
                if (ServiceMethodRegistry.ServiceMethods.Any(m => String.Equals(m_relativeUrl, m.Key.Url, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, RestResources.AlreadyMapped, m_relativeUrl));
                }

                ServiceMethodRegistry.ServiceMethods.AddOrUpdate(new ServiceMetadata(contractType, m_relativeUrl), t => methodMetadata, (t, u) => methodMetadata);
            }

            IEnumerable<IRestServiceHandler> routeHandlers = MapRoutes(methodMetadata, contractType);
            return new RouteConfiguration(routeHandlers);
        }

        private IEnumerable<IRestServiceHandler> MapRoutes(IEnumerable<ServiceMethodMetadata> methodMetadata, Type serviceContractType)
        {
            var routeHandlers = new List<IRestServiceHandler>();
            var orderedMethodMetadata = methodMetadata.OrderByDescending(m => m.UrlInfo.Priority);

            foreach (ServiceMethodMetadata metadata in orderedMethodMetadata)
            {
                var defaults = new RouteValueDictionary
                                   {
                                       { RouteConstants.ServiceContractType, serviceContractType.AssemblyQualifiedName },
                                       { RouteConstants.ServiceUrl, m_relativeUrl },
                                       { RouteConstants.UrlTemplate, metadata.UrlInfo.UrlTemplate.Trim() }
                                   };

                var constraints = new RouteValueDictionary
                {
                    { RouteConstants.RouteConstraint, new ServiceRouteConstraint(metadata) }
                };

                ConfigureOptionalRouteParameters(metadata, defaults);

                var routeHandler = Rest.Configuration.ServiceLocator.GetService<IRestServiceHandler>();

                if (m_serviceTimeout.HasValue)
                {
                    routeHandler.ServiceTimeout = m_serviceTimeout.Value;
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

            return routeHandlers;
        }

        private void SetBrowserRoutes(string serviceUrl, ServiceMethodMetadata metadata)
        {
            string externalUrl = metadata.UrlInfo.WebPageUrl.Trim();

            var constraints = new RouteValueDictionary
            {
                {
                    RouteConstants.BrowserConstraint, new BrowserRouteConstraint(Rest.Configuration.ServiceLocator.GetService<IContentNegotiator>(),
                                                                                 Rest.Configuration.ServiceLocator.GetService<IHttpRequest>())
                }
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
