// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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

        private readonly string m_relativeUrl;
        private readonly RouteCollection m_routes;
        private readonly TimeSpan? m_asyncTimeout;

        internal RouteBuilder(string relativeUrl, RouteCollection routes, TimeSpan? asyncTimeout)
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
            m_asyncTimeout = asyncTimeout;
        }

        /// <summary>
        /// Specifies that the URL should be mapped to the service using an asynchronous REST HTTP handler.
        /// It is only recommended to use asynchronous handlers for services that perform heavy operations
        /// that can block the worker process or cause the server to run out of the ASP .NET thread pool.
        /// This method has no effect on web forms pages. Use the <code>async="true"</code> directive to
        /// make web forms pages asynchronous.
        /// </summary>
        /// <returns>The route builder.</returns>
        public RouteBuilder WithAsyncHandler()
        {
            return new RouteBuilder(m_relativeUrl, m_routes, TimeSpan.Zero);
        }

        /// <summary>
        /// Specifies that the URL should be mapped to the service using an asynchronous REST HTTP handler.
        /// It is only recommended to use asynchronous handlers for services that perform heavy operations
        /// that can block the worker process or cause the server to run out of the ASP .NET thread pool.
        /// This method has no effect on web forms pages. Use the <code>async="true"</code> directive to
        /// make web forms pages asynchronous. In case of a time out, it may take additional time to cancel the operation.
        /// </summary>
        /// <param name="timeout">The async service method timeout.</param>
        /// <returns>The route builder.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the timeout is less than 1 second.</exception>
        public RouteBuilder WithAsyncHandler(TimeSpan timeout)
        {
            if (timeout.TotalMilliseconds < CancellationOperation.MinTimeoutInMilliseconds)
            {
                throw new ArgumentOutOfRangeException("timeout", timeout.TotalSeconds, RestResources.OutOfRangeAsyncServiceTimeout);
            }

            return new RouteBuilder(m_relativeUrl, m_routes, timeout);
        }

        /// <summary>
        /// Specifies that the URL should be mapped to the service using an asynchronous REST HTTP handler.
        /// It is only recommended to use asynchronous handlers for services that perform heavy operations
        /// that can block the worker process or cause the server to run out of the ASP .NET thread pool.
        /// This method has no effect on web forms pages. Use the <code>async="true"</code> directive to
        /// make web forms pages asynchronous. In case of a time out, it may take additional time to cancel the operation.
        /// </summary>
        /// <param name="timeoutInMilliseconds">The async service method timeout in milliseconds.</param>
        /// <returns>The route builder.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the timeout is less than 1 second.</exception>
        public RouteBuilder WithAsyncHandler(int timeoutInMilliseconds)
        {
            return WithAsyncHandler(TimeSpan.FromMilliseconds(timeoutInMilliseconds));
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

                    urlAttributes.Add(methodMetadata);

                    HttpMethodRegistry.HttpMethods.AddOrUpdate(new RouteMetadata(serviceContractType.AssemblyQualifiedName, urlAttribute.UrlTemplate),
                                                               template => AddHttpMethods(httpMethods),
                                                               (template, allowedMethods) => UpdateHttpMethods(allowedMethods, httpMethods));
                }
            }

            return urlAttributes;
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

            if (contractType.IsClass && (contractType.IsAbstract || Attribute.GetCustomAttribute(contractType, typeof(ServiceContractAttribute), true) == null))
            {
                throw new ArgumentException(RestResources.InvalidServiceImplementation, "contractType");
            }

            var serviceMethods = contractType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                             .Where(m => m.GetCustomAttributes(urlAttributeType, false).Length > 0);

            List<ServiceMethodMetadata> methodMetadata = GenerateMethodMetadata(contractType, serviceMethods, m_relativeUrl);

            lock (syncRoot)
            {
                if (ServiceMethodRegistry.ServiceMethods.Any(m => String.Equals(m_relativeUrl, m.Key.Url, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, RestResources.AlreadyMapped, m_relativeUrl));
                }

                ServiceMethodRegistry.ServiceMethods.AddOrUpdate(new ServiceMetadata(contractType, m_relativeUrl), t => methodMetadata, (t, u) => methodMetadata);
            }

            IEnumerable<IRestHandler> routeHandlers = MapRoutes(methodMetadata, contractType);
            return new RouteConfiguration(routeHandlers);
        }

        private IRestHandler CreateRouteHandler()
        {
            IRestHandler routeHandler;

            if (m_asyncTimeout.HasValue)
            {
                if (m_asyncTimeout.Value.TotalMilliseconds >= CancellationOperation.MinTimeoutInMilliseconds)
                {
                    routeHandler = Rest.Configuration.ServiceLocator.GetService<RestAsyncCancellableHandler>();
                    ((RestAsyncCancellableHandler) routeHandler).AsyncTimeout = m_asyncTimeout.Value;
                }
                else if (m_asyncTimeout.Value == TimeSpan.Zero)
                {
                    routeHandler = Rest.Configuration.ServiceLocator.GetService<RestAsyncHandler>();
                }
                else
                {
                    throw new InvalidOperationException(RestResources.InvalidAsyncServiceTimeout);
                }
            }
            else
            {
                routeHandler = Rest.Configuration.ServiceLocator.GetService<RestHandler>();
            }

            return routeHandler;
        }

        private IEnumerable<IRestHandler> MapRoutes(IEnumerable<ServiceMethodMetadata> methodMetadata, Type serviceContractType)
        {
            var routeHandlers = new List<IRestHandler>();
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

                IRestHandler routeHandler = CreateRouteHandler();
                routeHandlers.Add(routeHandler);

                string serviceUrl = ConcatUrl(m_relativeUrl, metadata.UrlInfo.UrlTemplate.Trim());

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
