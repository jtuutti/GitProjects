using System;
using System.Collections.Generic;

namespace RestFoundation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class UrlAttribute : Attribute
    {
        public UrlAttribute(string urlTemplate, params HttpMethod[] httpMethods)
        {
            if (urlTemplate == null) throw new ArgumentNullException("urlTemplate");

            UrlTemplate = urlTemplate.Trim();
            HttpMethods = (httpMethods != null && httpMethods.Length > 0) ? httpMethods : null;

            if (httpMethods != null && Array.IndexOf(httpMethods, HttpMethod.Options) >= 0)
            {
                throw new InvalidOperationException("HTTP method OPTIONS cannot be manually defined on a service method");
            }
        }

        public int Priority { get; set; }
        public string UrlTemplate { get; private set; }
        public IEnumerable<HttpMethod> HttpMethods { get; internal set; }
    }
}
