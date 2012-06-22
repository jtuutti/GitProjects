using System;
using System.Collections.Generic;

namespace RestFoundation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class UrlAttribute : Attribute
    {
        public UrlAttribute(string urlTemplate, params HttpMethod[] methods)
        {
            if (urlTemplate == null) throw new ArgumentNullException("urlTemplate");

            UrlTemplate = urlTemplate.Trim();
            HttpMethods = (methods != null && methods.Length > 0) ? methods : new[] { HttpMethod.Get, HttpMethod.Head };

            if (methods != null && Array.IndexOf(methods, HttpMethod.Options) >= 0)
            {
                throw new InvalidOperationException("HTTP method OPTIONS cannot be manually defined on a service method");
            }
        }

        public int Priority
        {
            get;
            set;
        }

        public string UrlTemplate
        {
            get;
            private set;
        }

        public IEnumerable<HttpMethod> HttpMethods
        {
            get;
            protected set;
        }
    }
}
