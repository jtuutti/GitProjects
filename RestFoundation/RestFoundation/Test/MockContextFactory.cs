using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Routing;

namespace RestFoundation.Test
{
    public sealed class MockContextFactory : IDisposable
    {
        private static readonly object syncRoot = new object();

        internal static HttpContextBase Context { get; private set; }

        public RequestContext Create<T>(string relativeUrl, Expression<Action<T>> serviceMethodDelegate)
        {
            return Create(relativeUrl, serviceMethodDelegate, null);
        }

        public RequestContext Create<T>(string relativeUrl, Expression<Action<T>> serviceMethodDelegate, HttpMethod? httpMethod)
        {
            if (serviceMethodDelegate == null) throw new ArgumentNullException("serviceMethodDelegate");
            if (String.IsNullOrEmpty(relativeUrl)) throw new ArgumentNullException("relativeUrl");

            if (!relativeUrl.StartsWith("~/"))
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

            if (urlAttribute == null || urlAttribute.HttpMethods == null || urlAttribute.UrlTemplate == null)
            {
                throw new ArgumentException("No valid service method provided", "serviceMethodDelegate");
            }

            if (httpMethod.HasValue)
            {
                if (httpMethod.Value != HttpMethod.Options && !urlAttribute.HttpMethods.Contains(httpMethod.Value))
                {
                    throw new ArgumentException("No supported HTTP method provided", "serviceMethodDelegate");
                }
            }
            else
            {
                httpMethod = urlAttribute.HttpMethods.First();
            }

            var routes = RouteTable.Routes;

            if (routes == null)
            {
                throw new InvalidOperationException("No active routes were found");
            }

            lock (syncRoot)
            {
                Context = new TestHttpContext(relativeUrl, httpMethod.Value.ToString().ToUpperInvariant());

                var routeData = routes.GetRouteData(Context);

                if (routeData == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound, "Not Found");
                }

                return new RequestContext(Context, routeData);
            }
        }

        public void Dispose()
        {
            lock (syncRoot)
            {
                Context = null;
            }
        }
    }
}
