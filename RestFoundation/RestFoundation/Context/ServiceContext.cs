// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Routing;
using RestFoundation.Runtime;

namespace RestFoundation.Context
{
    /// <summary>
    /// Represents a service context.
    /// </summary>
    public class ServiceContext : ContextBase, IServiceContext
    {
        private readonly IHttpRequest m_request;
        private readonly IHttpResponse m_response;
        private readonly IServiceCache m_cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceContext"/> class.
        /// </summary>
        /// <param name="request">The current HTTP request.</param>
        /// <param name="response">The current HTTP response.</param>
        /// <param name="cache">The service cache.</param>
        public ServiceContext(IHttpRequest request, IHttpResponse response, IServiceCache cache)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            m_request = request;
            m_response = response;
            m_cache = cache;
        }

        /// <summary>
        /// Gets the current HTTP request.
        /// </summary>
        public IHttpRequest Request
        {
            get
            {
                return m_request;
            }
        }

        /// <summary>
        /// Gets the current HTTP response.
        /// </summary>
        public IHttpResponse Response
        {
            get
            {
                return m_response;
            }
        }

        /// <summary>
        /// Gets the service cache.
        /// </summary>
        public IServiceCache Cache
        {
            get
            {
                return m_cache;
            }
        }

        /// <summary>
        /// Gets or sets a time span before a service times out.
        /// </summary>
        public TimeSpan ServiceTimeout
        {
            get
            {
                return TimeSpan.FromSeconds(Context.Server.ScriptTimeout);
            }
            set
            {
                Context.Server.ScriptTimeout = Convert.ToInt32(value.TotalSeconds);
            }
        }

        /// <summary>
        /// Gets or sets security information for the current HTTP request.
        /// </summary>
        public IPrincipal User
        {
            get
            {
                return Context.User;
            }
            set
            {
                Context.User = value;
                Thread.CurrentPrincipal = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is an authenticated user associated with the HTTP request.
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                return Context.User != null && Context.User.Identity.IsAuthenticated;
            }
        }

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(Expression<Action<TContract>> serviceMethod)
        {
            return GetPath(null, serviceMethod, new RouteValueDictionary(), HttpScheme.None);
        }

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values based on the object properties.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(Expression<Action<TContract>> serviceMethod, object routeValues)
        {
            return GetPath(null, serviceMethod, new RouteValueDictionary(routeValues), HttpScheme.None);
        }

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="UrlBuilder"/> configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod)
        {
            return GetPath(serviceUrl, serviceMethod, new RouteValueDictionary(), HttpScheme.None);
        }

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="UrlBuilder"/> configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values based on the object properties.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod, object routeValues)
        {
            return GetPath(serviceUrl, serviceMethod, routeValues, HttpScheme.None);
        }

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="UrlBuilder"/> configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod, RouteValueDictionary routeValues)
        {
            return GetPath(serviceUrl, serviceMethod, routeValues, HttpScheme.None);
        }

        /// <summary>
        /// Gets the absolute relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="UrlBuilder"/> configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values based on the object properties.</param>
        /// <param name="scheme">The protocol scheme to construct a full absolute URL.</param>
        /// <returns>The application absolute URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod, object routeValues, HttpScheme scheme)
        {
            return GetPath(serviceUrl, serviceMethod, new RouteValueDictionary(routeValues), scheme);
        }

        /// <summary>
        /// Gets the application absolute URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="UrlBuilder"/> configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <param name="scheme">The protocol scheme to construct a full absolute URL.</param>
        /// <returns>The application absolute URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod, RouteValueDictionary routeValues, HttpScheme scheme)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            var methodExpression = serviceMethod.Body as MethodCallExpression;

            if (methodExpression == null || methodExpression.Method == null)
            {
                throw new ArgumentException(RestResources.InvalidServiceMethodExpression, "serviceMethod");
            }

            if (serviceUrl == null)
            {
                serviceUrl = TryGetMatchingServiceUrl(methodExpression);
            }

            return GetPathFromMethodExpression(serviceUrl, methodExpression, routeValues, scheme);
        }

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(Expression<Func<TContract, object>> serviceMethod)
        {
            return GetPath(null, serviceMethod, new RouteValueDictionary(), HttpScheme.None);
        }

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values based on the object properties.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(Expression<Func<TContract, object>> serviceMethod, object routeValues)
        {
            return GetPath(null, serviceMethod, new RouteValueDictionary(routeValues), HttpScheme.None);
        }

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="UrlBuilder"/> configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod)
        {
            return GetPath(serviceUrl, serviceMethod, new RouteValueDictionary(), HttpScheme.None);
        }

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="UrlBuilder"/> configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values based on the object properties.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod, object routeValues)
        {
            return GetPath(serviceUrl, serviceMethod, routeValues, HttpScheme.None);
        }

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="UrlBuilder"/> configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod, RouteValueDictionary routeValues)
        {
            return GetPath(serviceUrl, serviceMethod, routeValues, HttpScheme.None);
        }

        /// <summary>
        /// Gets the application absolute URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="UrlBuilder"/> configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values based on the object properties.</param>
        /// <param name="scheme">The protocol scheme to construct a full absolute URL.</param>
        /// <returns>The application absolute URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod, object routeValues, HttpScheme scheme)
        {
            return GetPath(serviceUrl, serviceMethod, new RouteValueDictionary(routeValues), scheme);
        }

        /// <summary>
        /// Gets the application absolute URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="UrlBuilder"/> configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <param name="scheme">The protocol scheme to construct a full absolute URL.</param>
        /// <returns>The application absolute URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod, RouteValueDictionary routeValues, HttpScheme scheme)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            var methodExpression = serviceMethod.Body as MethodCallExpression;

            if (methodExpression == null || methodExpression.Method == null)
            {
                throw new InvalidOperationException(RestResources.InvalidServiceMethodExpression);
            }

            if (serviceUrl == null)
            {
                serviceUrl = TryGetMatchingServiceUrl(methodExpression);
            }

            return GetPathFromMethodExpression(serviceUrl, methodExpression, routeValues, scheme);
        }

        /// <summary>
        /// Returns the physical path associated to the virtual path of the file.
        /// </summary>
        /// <param name="relativePath">The relative virtual path to the file.</param>
        /// <returns>The physical file path.</returns>
        public string MapPath(string relativePath)
        {
            return Context.Server.MapPath(relativePath);
        }

        /// <summary>
        /// Gets the underlying <see cref="HttpContextBase"/> instance that may provide additional functionality.
        /// </summary>
        /// <returns>The underlying HTTP context instance.</returns>
        public HttpContextBase GetHttpContext()
        {
            return Context;
        }

        private string TryGetMatchingServiceUrl(MethodCallExpression methodExpression)
        {
            IList<ServiceMethodMetadata> serviceMethodList = ServiceMethodRegistry.GetMethodMetadata(methodExpression.Method);

            if (serviceMethodList.Count == 0)
            {
                throw new InvalidOperationException(RestResources.InvalidServiceMethodExpression);
            }

            if (serviceMethodList.Count > 1)
            {
                string serviceUrl;

                if (TryGetCurrentServiceUrl(serviceMethodList, out serviceUrl))
                {
                    return serviceUrl;
                }

                throw new InvalidOperationException(RestResources.MissingAmbiguousServiceUrl);
            }

            return serviceMethodList[0].ServiceUrl;
        }

        private bool TryGetCurrentServiceUrl(IEnumerable<ServiceMethodMetadata> serviceMethodList, out string serviceUrl)
        {
            var routeTable = RouteTable.Routes.GetRouteData(Context);

            if (routeTable == null)
            {
                serviceUrl = null;
                return false;
            }

            object currentServiceUrl;

            if (routeTable.Values.TryGetValue(RouteConstants.ServiceUrl, out currentServiceUrl) && currentServiceUrl != null)
            {
                var serviceMethod = serviceMethodList.FirstOrDefault(m => String.Equals(currentServiceUrl.ToString(), m.ServiceUrl, StringComparison.OrdinalIgnoreCase));

                if (!String.IsNullOrEmpty(serviceMethod.ServiceUrl))
                {
                    serviceUrl = serviceMethod.ServiceUrl;
                    return true;
                }
            }

            serviceUrl = null;
            return false;
        }

        private string GetPathFromMethodExpression(string serviceUrl, MethodCallExpression methodExpression, RouteValueDictionary routeValues, HttpScheme scheme)
        {
            string routeName = RouteNameHelper.GetRouteName(serviceUrl, methodExpression.Method);

            if (String.IsNullOrEmpty(routeName))
            {
                throw new InvalidOperationException(RestResources.InvalidServiceUrlOrMethodExpression);
            }

            IList<ExpressionArgument> arguments = ExpressionArgumentExtractor.Extract(methodExpression);

            foreach (var argument in arguments)
            {
                if (!routeValues.ContainsKey(argument.Name))
                {
                    routeValues[argument.Name] = argument.Value;
                }
            }

            return GetPathFromRoute(routeValues, routeName, scheme);
        }

        private string GetPathFromRoute(RouteValueDictionary routeValues, string routeName, HttpScheme scheme)
        {
            RouteData routeData = RouteTable.Routes.GetRouteData(Context);

            if (routeData == null)
            {
                throw new InvalidOperationException(RestResources.MissingRouteData);
            }

            var requestContext = new RequestContext(Context, routeData);
            VirtualPathData methodRoute;

            try
            {
                methodRoute = RouteTable.Routes.GetVirtualPath(requestContext, routeName, routeValues);
            }
            catch (ArgumentException)
            {
                methodRoute = null;
            }

            if (methodRoute == null || String.IsNullOrWhiteSpace(methodRoute.VirtualPath))
            {
                throw new InvalidOperationException(RestResources.InvalidServiceUrlOrMethodExpression);
            }

            string relativeUrl = VirtualPathUtility.ToAbsolute(methodRoute.VirtualPath, Context.Request.ApplicationPath);

            if (Context.Request.Url == null)
            {
                return relativeUrl;
            }

            switch (scheme)
            {
                case HttpScheme.Http:
                    return String.Format(CultureInfo.InvariantCulture, "http://{0}{1}", Context.Request.Url.Host, relativeUrl);
                case HttpScheme.Https:
                    return String.Format(CultureInfo.InvariantCulture, "https://{0}{1}", Context.Request.Url.Host, relativeUrl);
                default:
                    return relativeUrl;
            }
        }
    }
}
