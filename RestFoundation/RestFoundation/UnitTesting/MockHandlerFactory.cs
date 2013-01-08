// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Routing;
using System.Xml.Serialization;
using RestFoundation.Client;
using RestFoundation.Runtime;
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
        /// Creates a REST handler that executes that provided service method at the specified virtual URL.
        /// </summary>
        /// <typeparam name="T">The service contract type.</typeparam>
        /// <param name="virtualUrl">The virtual service URL.</param>
        /// <param name="serviceMethodDelegate">The service method delegate.</param>
        /// <returns>The REST handler./</returns>
        public IRestHandler Create<T>(string virtualUrl, Expression<Action<T>> serviceMethodDelegate)
        {
            return Create(virtualUrl, serviceMethodDelegate, null);
        }

        /// <summary>
        /// Creates a REST handler that executes that provided service method at the specified virtual URL.
        /// </summary>
        /// <typeparam name="T">The service contract type.</typeparam>
        /// <param name="virtualUrl">The virtual service URL.</param>
        /// <param name="serviceMethodDelegate">The service method delegate.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <returns>The REST handler./</returns>
        public IRestHandler Create<T>(string virtualUrl, Expression<Action<T>> serviceMethodDelegate, HttpMethod? httpMethod)
        {
            if (serviceMethodDelegate == null)
            {
                throw new ArgumentNullException("serviceMethodDelegate");
            }

            if (String.IsNullOrEmpty(virtualUrl))
            {
                throw new ArgumentNullException("virtualUrl");
            }

            RequestContext requestContext = CreateContext(virtualUrl, serviceMethodDelegate, httpMethod);

            return new MockRestHandler().GetHttpHandler(requestContext) as IRestHandler;
        }

        /// <summary>
        /// Creates an asyncronous REST handler that executes that provided service method at the specified virtual URL.
        /// </summary>
        /// <typeparam name="T">The service contract type.</typeparam>
        /// <param name="virtualUrl">The virtual service URL.</param>
        /// <param name="serviceMethodDelegate">The service method delegate.</param>
        /// <returns>The asynchronous REST handler./</returns>
        public IRestAsyncHandler CreateAsync<T>(string virtualUrl, Expression<Action<T>> serviceMethodDelegate)
        {
            return CreateAsync(virtualUrl, serviceMethodDelegate, null);
        }

        /// <summary>
        /// Creates an asyncronous REST handler that executes that provided service method at the specified virtual URL.
        /// </summary>
        /// <typeparam name="T">The service contract type.</typeparam>
        /// <param name="virtualUrl">The virtual service URL.</param>
        /// <param name="serviceMethodDelegate">The service method delegate.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <returns>The asynchronous REST handler./</returns>
        public IRestAsyncHandler CreateAsync<T>(string virtualUrl, Expression<Action<T>> serviceMethodDelegate, HttpMethod? httpMethod)
        {
            if (serviceMethodDelegate == null)
            {
                throw new ArgumentNullException("serviceMethodDelegate");
            }

            if (String.IsNullOrEmpty(virtualUrl))
            {
                throw new ArgumentNullException("virtualUrl");
            }

            RequestContext requestContext = CreateContext(virtualUrl, serviceMethodDelegate, httpMethod);

            return new MockRestAsyncHandler().GetHttpHandler(requestContext) as IRestAsyncHandler;
        }

        /// <summary>
        /// Sets a response object of the provided type as the body of the request.
        /// </summary>
        /// <param name="resource">The resource object.</param>
        /// <param name="resourceType">The resource type (JSON or XML).</param>
        /// <exception cref="InvalidOperationException">If the HTTP method does not support resources/body content.</exception>
        public void SetResource(object resource, RestResourceType resourceType)
        {
            if (resource == null)
            {
                throw new ArgumentNullException("resource");
            }

            if (!String.Equals(Context.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase) &&
                !String.Equals(Context.Request.HttpMethod, "PUT", StringComparison.OrdinalIgnoreCase) &&
                !String.Equals(Context.Request.HttpMethod, "PATCH", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(RestResources.InvalidHttpMethodForResource);
            }

            if (resourceType == RestResourceType.Json)
            {
                Context.Request.Headers.Add("Content-Type", "application/json; charset=utf-8");

                var serializer = JsonSerializerFactory.Create();
                var writer = new StreamWriter(Context.Request.InputStream, Encoding.UTF8);
                serializer.Serialize(writer, resource);
                writer.Flush();
            }
            else
            {
                Context.Request.Headers.Add("Content-Type", "application/xml; charset=utf-8");

                var serializer = XmlSerializerRegistry.Get(resource.GetType());
                var writer = new StreamWriter(Context.Request.InputStream, Encoding.UTF8);
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(String.Empty, XmlNameSpaceExtractor.Get());
                serializer.Serialize(writer, resource, namespaces);
                writer.Flush();
            }
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

        private static RequestContext CreateContext<T>(string virtualUrl, Expression<Action<T>> serviceMethodDelegate, HttpMethod? httpMethod)
        {
            if (!virtualUrl.StartsWith("~/", StringComparison.Ordinal))
            {
                throw new ArgumentException(RestResources.InvalidVirtualUrl);
            }

            Type contractType = typeof(T);

            if (!contractType.IsInterface && !contractType.IsClass)
            {
                throw new ArgumentException(RestResources.InvalidServiceContract, "serviceMethodDelegate");
            }

            if (contractType.IsClass && (contractType.IsAbstract || !Attribute.IsDefined(contractType, typeof(ServiceContractAttribute), true)))
            {
                throw new ArgumentException(RestResources.InvalidServiceImplementation, "serviceMethodDelegate");
            }

            var serviceMethodCallDelegate = serviceMethodDelegate.Body as MethodCallExpression;

            if (serviceMethodCallDelegate == null)
            {
                throw new ArgumentException(RestResources.InvalidServiceMethodExpression, "serviceMethodDelegate");
            }

            var serviceMethod = serviceMethodCallDelegate.Method;

            if (serviceMethod == null)
            {
                throw new ArgumentException(RestResources.InvalidServiceMethod, "serviceMethodDelegate");
            }

            var urlAttribute = Attribute.GetCustomAttribute(serviceMethod, typeof(UrlAttribute), false) as UrlAttribute;

            if (urlAttribute == null || urlAttribute.UrlTemplate == null)
            {
                throw new RouteAssertException(RestResources.InvalidServiceMethod);
            }

            if (urlAttribute.HttpMethods == null)
            {
                urlAttribute.HttpMethods = Rest.Configuration.ServiceLocator.GetService<IHttpMethodResolver>().Resolve(serviceMethod);

                if (urlAttribute.HttpMethods == null)
                {
                    throw new RouteAssertException(RestResources.DisallowedHttpMethod);
                }
            }

            if (httpMethod.HasValue)
            {
                if (httpMethod.Value != HttpMethod.Options && !urlAttribute.HttpMethods.Contains(httpMethod.Value))
                {
                    throw new RouteAssertException(RestResources.DisallowedHttpMethod);
                }
            }
            else
            {
                httpMethod = urlAttribute.HttpMethods.First();
            }

            return GenerateRouteContext(virtualUrl, httpMethod.Value, serviceMethod);
        }

        private static RequestContext GenerateRouteContext(string virtualUrl, HttpMethod httpMethod, MethodInfo serviceMethod)
        {
            RouteCollection routes = RouteTable.Routes;

            if (routes == null)
            {
                throw new RouteAssertException(RestResources.MissingRouteData);
            }

            lock (syncRoot)
            {
                Context = new TestHttpContext(virtualUrl, httpMethod.ToString().ToUpperInvariant());

                RouteData routeData = routes.GetRouteData(Context);

                if (routeData == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound, RestResources.NotFound);
                }

                AssertRouteParameters(serviceMethod, routeData);

                return new RequestContext(Context, routeData);
            }
        }

        private static void AssertRouteParameters(MethodInfo serviceMethod, RouteData routeData)
        {
            foreach (var routeParameter in routeData.Values)
            {
                if (routeParameter.Key.StartsWith("_", StringComparison.Ordinal))
                {
                    continue;
                }

                if (!serviceMethod.GetParameters().Select(p => p.Name).Contains(routeParameter.Key, StringComparer.OrdinalIgnoreCase))
                {
                    throw new RouteAssertException(RestResources.MismatchedServiceMethodRoute);
                }
            }
        }
    }
}
