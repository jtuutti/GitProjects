using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Web.Routing;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Represents a mock handler factory.
    /// </summary>
    public sealed class MockHandlerFactory : IDisposable
    {
        private static readonly object syncRoot = new object();

        internal static TestHttpContext Context { get; private set; }

        /// <summary>
        /// Creates a REST handler that executes that provided service method at the specified relative URL.
        /// </summary>
        /// <typeparam name="T">The service contract type.</typeparam>
        /// <param name="relativeUrl">The relative URL.</param>
        /// <param name="serviceMethodDelegate">The service method delegate.</param>
        /// <returns>The REST handler./</returns>
        public IRestHandler Create<T>(string relativeUrl, Expression<Action<T>> serviceMethodDelegate)
        {
            return Create(relativeUrl, serviceMethodDelegate, null);
        }

        /// <summary>
        /// Creates a REST handler that executes that provided service method at the specified relative URL.
        /// </summary>
        /// <typeparam name="T">The service contract type.</typeparam>
        /// <param name="relativeUrl">The relative URL.</param>
        /// <param name="serviceMethodDelegate">The service method delegate.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <returns>The REST handler./</returns>
        public IRestHandler Create<T>(string relativeUrl, Expression<Action<T>> serviceMethodDelegate, HttpMethod? httpMethod)
        {
            if (serviceMethodDelegate == null) throw new ArgumentNullException("serviceMethodDelegate");
            if (String.IsNullOrEmpty(relativeUrl)) throw new ArgumentNullException("relativeUrl");

            RequestContext requestContext = CreateContext(relativeUrl, serviceMethodDelegate, httpMethod);

            return new MockRestHandler().GetHttpHandler(requestContext) as IRestHandler;
        }

        /// <summary>
        /// Creates an asyncronous REST handler that executes that provided service method at the specified relative URL.
        /// </summary>
        /// <typeparam name="T">The service contract type.</typeparam>
        /// <param name="relativeUrl">The relative URL.</param>
        /// <param name="serviceMethodDelegate">The service method delegate.</param>
        /// <returns>The asynchronous REST handler./</returns>
        public IRestAsyncHandler CreateAsync<T>(string relativeUrl, Expression<Action<T>> serviceMethodDelegate)
        {
            return CreateAsync(relativeUrl, serviceMethodDelegate, null);
        }

        /// <summary>
        /// Creates an asyncronous REST handler that executes that provided service method at the specified relative URL.
        /// </summary>
        /// <typeparam name="T">The service contract type.</typeparam>
        /// <param name="relativeUrl">The relative URL.</param>
        /// <param name="serviceMethodDelegate">The service method delegate.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <returns>The asynchronous REST handler./</returns>
        public IRestAsyncHandler CreateAsync<T>(string relativeUrl, Expression<Action<T>> serviceMethodDelegate, HttpMethod? httpMethod)
        {
            if (serviceMethodDelegate == null) throw new ArgumentNullException("serviceMethodDelegate");
            if (String.IsNullOrEmpty(relativeUrl)) throw new ArgumentNullException("relativeUrl");

            RequestContext requestContext = CreateContext(relativeUrl, serviceMethodDelegate, httpMethod);

            return new MockRestAsyncHandler().GetHttpHandler(requestContext) as IRestAsyncHandler;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (Context == null)
            {
                return;
            }

            lock (syncRoot)
            {
                if (Context == null)
                {
                    return;
                }

                Context.Dispose();
                Context = null;
            }
        }

        private static RequestContext CreateContext<T>(string relativeUrl, Expression<Action<T>> serviceMethodDelegate, HttpMethod? httpMethod)
        {
            if (!relativeUrl.StartsWith("~/", StringComparison.Ordinal))
            {
                throw new ArgumentException("Relative URL must start with ~/");
            }

            Type serviceContractType = typeof(T);

            if (!serviceContractType.IsInterface)
            {
                throw new ArgumentException("Service contract type must be an interface", "serviceMethodDelegate");
            }

            var serviceMethodCallDelegate = serviceMethodDelegate.Body as MethodCallExpression;

            if (serviceMethodCallDelegate == null)
            {
                throw new ArgumentException("No valid service method lambda expression provided", "serviceMethodDelegate");
            }

            var serviceMethod = serviceMethodCallDelegate.Method;

            if (serviceMethod == null)
            {
                throw new ArgumentException("No valid service method rovided", "serviceMethodDelegate");
            }

            var urlAttribute = Attribute.GetCustomAttribute(serviceMethod, typeof(UrlAttribute), false) as UrlAttribute;

            if (urlAttribute == null || urlAttribute.UrlTemplate == null)
            {
                throw new RouteAssertException("No valid service method provided");
            }

            if (urlAttribute.HttpMethods == null)
            {
                urlAttribute.HttpMethods = Rest.Active.ServiceLocator.GetService<IHttpMethodResolver>().Resolve(serviceMethod);

                if (urlAttribute.HttpMethods == null)
                {
                    throw new RouteAssertException("No supported HTTP method provided");
                }
            }

            if (httpMethod.HasValue)
            {
                if (httpMethod.Value != HttpMethod.Options && !urlAttribute.HttpMethods.Contains(httpMethod.Value))
                {
                    throw new RouteAssertException("No supported HTTP method provided");
                }
            }
            else
            {
                httpMethod = urlAttribute.HttpMethods.First();
            }

            return GenerateRouteContext(relativeUrl, httpMethod.Value, serviceMethod);
        }

        private static RequestContext GenerateRouteContext(string relativeUrl, HttpMethod httpMethod, MethodInfo serviceMethod)
        {
            RouteCollection routes = RouteTable.Routes;

            if (routes == null)
            {
                throw new RouteAssertException("No active routes were found");
            }

            lock (syncRoot)
            {
                Context = new TestHttpContext(relativeUrl, httpMethod.ToString().ToUpperInvariant());

                RouteData routeData = routes.GetRouteData(Context);

                if (routeData == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound, "Not Found");
                }

                foreach (var routeParameter in routeData.Values)
                {
                    if (routeParameter.Key.StartsWith("_", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    if (!serviceMethod.GetParameters().Select(p => p.Name).Contains(routeParameter.Key, StringComparer.OrdinalIgnoreCase))
                    {
                        throw new RouteAssertException("Provided service method delegate does not match the route.");
                    }
                }

                return new RequestContext(Context, routeData);
            }
        }
    }
}
