using System;
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

        public static HashSet<HttpMethod> GetHttpMethods(RouteMetadata actionMetadata)
        {
            if (actionMetadata == null) throw new ArgumentNullException("actionMetadata");

            HashSet<HttpMethod> allowedMethods;
            return httpMethods.TryGetValue(actionMetadata, out allowedMethods) ? allowedMethods : new HashSet<HttpMethod>();
        }
    }
}
