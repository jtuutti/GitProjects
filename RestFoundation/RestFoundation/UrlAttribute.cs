using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RestFoundation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class UrlAttribute : Attribute
    {
        public UrlAttribute(string urlTemplate, params HttpMethod[] httpMethods)
        {
            if (urlTemplate == null) throw new ArgumentNullException("urlTemplate");

            UrlTemplate = urlTemplate.Trim();
            HttpMethods = (httpMethods != null && httpMethods.Length > 0) ? httpMethods.Distinct() : null;

            if (httpMethods != null && Array.IndexOf(httpMethods, HttpMethod.Options) >= 0)
            {
                throw new InvalidOperationException("HTTP method OPTIONS cannot be manually defined on a service method");
            }
        }

        public UrlAttribute(string urlTemplate, string httpMethods)
        {
            if (urlTemplate == null) throw new ArgumentNullException("urlTemplate");

            UrlTemplate = urlTemplate.Trim();

            if (String.IsNullOrEmpty(httpMethods))
            {
                return;
            }

            string[] httpMethodArray = httpMethods.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<HttpMethod> httpMethodList = ParseHttpMethodsString(httpMethodArray);

            HttpMethods = (httpMethodList.Count > 0) ? httpMethodList : null;
        }

        public int Priority { get; set; }
        public string UrlTemplate { get; private set; }
        public IEnumerable<HttpMethod> HttpMethods { get; internal set; }
        public string WebPageRelativePath { get; set; }

        private static List<HttpMethod> ParseHttpMethodsString(string[] httpMethodArray)
        {
            var httpMethodList = new List<HttpMethod>();

            for (int i = 0; i < httpMethodArray.Length; i++)
            {
                HttpMethod httpMethod;

                if (!Enum.TryParse(httpMethodArray[i], true, out httpMethod))
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "HTTP Method '{0}' is not supported", httpMethodArray[i]));
                }

                if (httpMethod == HttpMethod.Options)
                {
                    throw new InvalidOperationException("HTTP method OPTIONS cannot be manually defined on a service method");
                }

                if (!httpMethodList.Contains(httpMethod))
                {
                    httpMethodList.Add(httpMethod);
                }
            }

            return httpMethodList;
        }
    }
}
