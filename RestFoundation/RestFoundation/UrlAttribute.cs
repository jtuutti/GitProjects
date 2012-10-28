// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RestFoundation
{
    /// <summary>
    /// Represents URL metadata associated with the service methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class UrlAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UrlAttribute"/> class with the url template
        /// and HTTP methods.
        /// </summary>
        /// <param name="urlTemplate">The URL template.</param>
        /// <param name="httpMethods">An array of HTTP methods.</param>
        /// <exception cref="InvalidOperationException">If the OPTIONS HTTP method is provided explicitly.</exception>
        public UrlAttribute(string urlTemplate, params HttpMethod[] httpMethods)
        {
            if (urlTemplate == null)
            {
                throw new ArgumentNullException("urlTemplate");
            }

            if (httpMethods != null && Array.IndexOf(httpMethods, HttpMethod.Options) >= 0)
            {
                throw new InvalidOperationException(RestResources.ManuallyDefinedOptionsHttpMethod);
            }

            UrlTemplate = urlTemplate.Trim();
            HttpMethods = (httpMethods != null && httpMethods.Length > 0) ? httpMethods.Distinct() : null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlAttribute"/> class with the url template
        /// and HTTP methods.
        /// </summary>
        /// <param name="urlTemplate">The URL template.</param>
        /// <param name="httpMethods">An comma separated list of HTTP methods as a <see cref="String"/>.</param>
        /// <exception cref="InvalidOperationException">If the OPTIONS HTTP method is provided explicitly.</exception>
        public UrlAttribute(string urlTemplate, string httpMethods)
        {
            if (urlTemplate == null)
            {
                throw new ArgumentNullException("urlTemplate");
            }

            UrlTemplate = urlTemplate.Trim();

            if (String.IsNullOrEmpty(httpMethods))
            {
                return;
            }

            string[] httpMethodArray = httpMethods.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<HttpMethod> httpMethodList = ParseHttpMethodsString(httpMethodArray);

            HttpMethods = (httpMethodList.Count > 0) ? httpMethodList : null;
        }

        /// <summary>
        /// Gets or sets the URL priority. A larger priority puts the URL route higher in the route collection.
        /// 0 is the default priority.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets the URL template.
        /// </summary>
        public string UrlTemplate { get; private set; }

        /// <summary>
        /// Gets a sequence of allowed HTTP methods.
        /// </summary>
        public IEnumerable<HttpMethod> HttpMethods { get; internal set; }

        /// <summary>
        /// Gets or sets the supporting web page URL. It can be a virtual path to an existing
        /// web forms page in the project or an external URL. This parameter is only used for
        /// service methods that are called through the HTTP GET or HEAD methods.
        /// </summary>
        public string WebPageUrl { get; set; }

        private static List<HttpMethod> ParseHttpMethodsString(string[] httpMethodArray)
        {
            var httpMethodList = new List<HttpMethod>();

            for (int i = 0; i < httpMethodArray.Length; i++)
            {
                HttpMethod httpMethod;

                if (!Enum.TryParse(httpMethodArray[i], true, out httpMethod))
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, RestResources.UnsupportedHttpMethod, httpMethodArray[i]));
                }

                if (httpMethod == HttpMethod.Options)
                {
                    throw new InvalidOperationException(RestResources.ManuallyDefinedOptionsHttpMethod);
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
