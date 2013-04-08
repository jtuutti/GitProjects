// <copyright>
// Dmitry Starosta, 2012-2013
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
        /// Gets a value indicating whether the service is running in debug mode.
        /// </summary>
        public bool IsDebug
        {
            get
            {
                return Context.IsDebuggingEnabled;
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
            return GetPath(null, serviceMethod, new RouteHash(), null);
        }

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(Expression<Action<TContract>> serviceMethod, RouteHash routeValues)
        {
            return GetPath(null, serviceMethod, routeValues, null);
        }

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="RestFoundation.Configuration.UrlBuilder"/>
        /// configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod)
        {
            return GetPath(serviceUrl, serviceMethod, new RouteHash(), null);
        }

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="RestFoundation.Configuration.UrlBuilder"/>
        /// configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod, RouteHash routeValues)
        {
            return GetPath(serviceUrl, serviceMethod, routeValues, null);
        }

        /// <summary>
        /// Gets the application absolute URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="RestFoundation.Configuration.UrlBuilder"/>
        /// configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <param name="segments">URI segments necessary to generate an absolute URL. Set it to null to generate a relative URL.</param>
        /// <returns>The application absolute URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod, RouteHash routeValues, UriSegments segments)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            Type contractType = typeof(TContract);

            if (!ServiceContractTypeRegistry.IsServiceContract(contractType))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Resources.Global.InvalidServiceContractType, contractType.Name));
            }

            var methodExpression = serviceMethod.Body as MethodCallExpression;

            if (methodExpression == null || methodExpression.Method == null)
            {
                throw new ArgumentException(Resources.Global.InvalidServiceMethodExpression, "serviceMethod");
            }

            if (serviceUrl == null)
            {
                serviceUrl = TryGetMatchingServiceUrl(methodExpression);
            }

            return GetPathFromMethodExpression(serviceUrl, methodExpression, routeValues, segments);
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
            return GetPath(null, serviceMethod, new RouteHash(), null);
        }

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(Expression<Func<TContract, object>> serviceMethod, RouteHash routeValues)
        {
            return GetPath(null, serviceMethod, routeValues, null);
        }

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="RestFoundation.Configuration.UrlBuilder"/>
        /// configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod)
        {
            return GetPath(serviceUrl, serviceMethod, new RouteHash(), null);
        }

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="RestFoundation.Configuration.UrlBuilder"/>
        /// configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod, RouteHash routeValues)
        {
            return GetPath(serviceUrl, serviceMethod, routeValues, null);
        }

        /// <summary>
        /// Gets the application absolute URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="RestFoundation.Configuration.UrlBuilder"/>
        /// configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <param name="segments">URI segments necessary to generate an absolute URL. Set it to null to generate a relative URL.</param>
        /// <returns>The application absolute URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        public string GetPath<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod, RouteHash routeValues, UriSegments segments)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            Type contractType = typeof(TContract);

            if (!ServiceContractTypeRegistry.IsServiceContract(contractType))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Resources.Global.InvalidServiceContractType, contractType.Name));
            }

            var methodExpression = serviceMethod.Body as MethodCallExpression;

            if (methodExpression == null || methodExpression.Method == null)
            {
                throw new InvalidOperationException(Resources.Global.InvalidServiceMethodExpression);
            }

            if (serviceUrl == null)
            {
                serviceUrl = TryGetMatchingServiceUrl(methodExpression);
            }

            return GetPathFromMethodExpression(serviceUrl, methodExpression, routeValues, segments);
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
                throw new InvalidOperationException(Resources.Global.InvalidServiceMethodExpression);
            }

            if (serviceMethodList.Count > 1)
            {
                string serviceUrl;

                if (TryGetCurrentServiceUrl(serviceMethodList, out serviceUrl))
                {
                    return serviceUrl;
                }

                throw new InvalidOperationException(Resources.Global.MissingAmbiguousServiceUrl);
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

            if (routeTable.Values.TryGetValue(ServiceCallConstants.ServiceUrl, out currentServiceUrl) && currentServiceUrl != null)
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

        private string GetPathFromMethodExpression(string serviceUrl, MethodCallExpression methodExpression, RouteHash routeValues, UriSegments segments)
        {
            string routeName = RouteNameHelper.GetRouteName(serviceUrl, methodExpression.Method);

            if (String.IsNullOrEmpty(routeName))
            {
                throw new InvalidOperationException(Resources.Global.InvalidServiceUrlOrMethodExpression);
            }

            if (routeValues == null)
            {
                routeValues = new RouteHash();
            }

            IList<ExpressionArgument> arguments = ExpressionArgumentExtractor.Extract(methodExpression);

            foreach (var argument in arguments)
            {
                if (!routeValues.ContainsKey(argument.Name))
                {
                    routeValues[argument.Name] = argument.Value;
                }
            }

            return GetPathFromRoute(routeValues, routeName, segments);
        }

        private string GetPathFromRoute(RouteHash routeValues, string routeName, UriSegments segments)
        {
            RouteData routeData = RouteTable.Routes.GetRouteData(Context);

            if (routeData == null)
            {
                throw new InvalidOperationException(Resources.Global.MissingRouteData);
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
                throw new InvalidOperationException(Resources.Global.InvalidServiceUrlOrMethodExpression);
            }

            string relativeUrl = VirtualPathUtility.ToAbsolute(methodRoute.VirtualPath, Context.Request.ApplicationPath);

            if (segments == null || Context.Request.Url == null)
            {
                return relativeUrl;
            }

            return segments.GenerateUri(Context.Request.Url.Host, relativeUrl);
        }
    }
}
