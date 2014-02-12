// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Routing;
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

        /// <summary>
        /// Creates a REST handler that executes that provided service method at the specified virtual URL.
        /// </summary>
        /// <typeparam name="T">The service contract type.</typeparam>
        /// <param name="virtualUrl">The virtual service URL.</param>
        /// <param name="serviceMethodDelegate">The service method delegate.</param>
        /// <returns>The REST handler./</returns>
        public IRestServiceHandler Create<T>(string virtualUrl, Expression<Action<T>> serviceMethodDelegate)
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
        public IRestServiceHandler Create<T>(string virtualUrl, Expression<Action<T>> serviceMethodDelegate, HttpMethod? httpMethod)
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

            return new MockRestHandler().GetHttpHandler(requestContext) as IRestServiceHandler;
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

            var context = TestHttpContext.Context;

            if (!String.Equals(context.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase) &&
                !String.Equals(context.Request.HttpMethod, "PUT", StringComparison.OrdinalIgnoreCase) &&
                !String.Equals(context.Request.HttpMethod, "PATCH", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(Resources.Global.InvalidHttpMethodForResource);
            }

            if (resourceType == RestResourceType.Json)
            {
                context.Request.Headers.Add("Content-Type", "application/json; charset=utf-8");

                var serializer = JsonSerializerFactory.Create();
                var writer = new StreamWriter(context.Request.InputStream, Encoding.UTF8);
                serializer.Serialize(writer, resource);
                writer.Flush();
            }
            else
            {
                context.Request.Headers.Add("Content-Type", "application/xml; charset=utf-8");

                var serializer = XmlSerializerRegistry.Get(resource.GetType());
                var writer = new StreamWriter(context.Request.InputStream, Encoding.UTF8);
                serializer.Serialize(writer, resource, XmlNamespaceManager.Generate());
                writer.Flush();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (TestHttpContext.Context == null)
            {
                return;
            }

            lock (syncRoot)
            {
                if (TestHttpContext.Context == null)
                {
                    return;
                }

                TestHttpContext.Context.Dispose();
                TestHttpContext.Context = null;
            }
        }

        private static RequestContext CreateContext<T>(string virtualUrl, Expression<Action<T>> serviceMethodDelegate, HttpMethod? httpMethod)
        {
            if (!virtualUrl.StartsWith("~/", StringComparison.Ordinal))
            {
                throw new ArgumentException(Resources.Global.InvalidVirtualUrl);
            }

            Type contractType = typeof(T);

            if (!contractType.IsInterface && !contractType.IsClass)
            {
                throw new ArgumentException(Resources.Global.InvalidServiceContract, "serviceMethodDelegate");
            }

            if (contractType.IsClass && (contractType.IsAbstract || !Attribute.IsDefined(contractType, typeof(ServiceContractAttribute), true)))
            {
                throw new ArgumentException(Resources.Global.InvalidServiceImplementation, "serviceMethodDelegate");
            }

            var serviceMethodCallDelegate = serviceMethodDelegate.Body as MethodCallExpression;

            if (serviceMethodCallDelegate == null)
            {
                throw new ArgumentException(Resources.Global.InvalidServiceMethodExpression, "serviceMethodDelegate");
            }

            var serviceMethod = serviceMethodCallDelegate.Method;

            if (serviceMethod == null)
            {
                throw new ArgumentException(Resources.Global.InvalidServiceMethod, "serviceMethodDelegate");
            }

            var urlAttribute = serviceMethod.GetCustomAttribute<UrlAttribute>(false);

            if (urlAttribute == null || urlAttribute.UrlTemplate == null)
            {
                throw new RouteAssertException(Resources.Global.InvalidServiceMethod);
            }

            if (urlAttribute.HttpMethods == null)
            {
                urlAttribute.HttpMethods = Rest.Configuration.ServiceLocator.GetService<IHttpMethodResolver>().Resolve(serviceMethod);

                if (urlAttribute.HttpMethods == null)
                {
                    throw new RouteAssertException(Resources.Global.DisallowedHttpMethod);
                }
            }

            if (httpMethod.HasValue)
            {
                if (httpMethod.Value != HttpMethod.Options && !urlAttribute.HttpMethods.Contains(httpMethod.Value))
                {
                    throw new RouteAssertException(Resources.Global.DisallowedHttpMethod);
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
                throw new RouteAssertException(Resources.Global.MissingRouteData);
            }

            lock (syncRoot)
            {
                TestHttpContext.Context = new TestHttpContext(virtualUrl, httpMethod.ToString().ToUpperInvariant());

                RouteData routeData = routes.GetRouteData(TestHttpContext.Context);

                if (routeData == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound, Resources.Global.NotFound);
                }

                AssertRouteParameters(serviceMethod, routeData);

                return new RequestContext(TestHttpContext.Context, routeData);
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
                    throw new RouteAssertException(Resources.Global.MismatchedServiceMethodRoute);
                }
            }
        }
    }
}
