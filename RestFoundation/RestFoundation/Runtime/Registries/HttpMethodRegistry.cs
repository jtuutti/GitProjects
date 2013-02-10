// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RestFoundation.Runtime
{
    internal static class HttpMethodRegistry
    {
        private static readonly ConcurrentDictionary<RouteMetadata, HashSet<HttpMethod>> httpMethods = new ConcurrentDictionary<RouteMetadata, HashSet<HttpMethod>>();

        public static ConcurrentDictionary<RouteMetadata, HashSet<HttpMethod>> HttpMethods
        {
            get
            {
                return httpMethods;
            }
        }

        public static ICollection<HttpMethod> GetHttpMethods(RouteMetadata metadata)
        {
            HashSet<HttpMethod> allowedMethods;
            return httpMethods.TryGetValue(metadata, out allowedMethods) ? allowedMethods : new HashSet<HttpMethod>();
        }
    }
}
