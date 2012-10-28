// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using RestFoundation.Results;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a service proxy operation.
    /// </summary>
    public sealed class ProxyOperation : IComparable<ProxyOperation>
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the operation has a response.
        /// </summary>
        public bool HasResource { get; set; }

        /// <summary>
        /// Gets or sets the HTTP method.
        /// </summary>
        public HttpMethod HttpMethod { get; set; }

        /// <summary>
        /// Gets or sets the HTTPS port, if applicable.
        /// </summary>
        public int HttpsPort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the operation is IP filtered.
        /// </summary>
        public bool IsIPFiltered { get; set; }

        /// <summary>
        /// Gets or sets the metadata URL.
        /// </summary>
        public string MetadataUrl { get; set; }

        /// <summary>
        /// Gets or sets the service proxy UI URL.
        /// </summary>
        public string ProxyUrl { get; set; }

        /// <summary>
        /// Gets or sets a value how many times the current template is repeated in the list.
        /// </summary>
        public int RepeatedTemplateCount { get; set; }

        /// <summary>
        /// Gets or sets the HTTP request resource example type or null.
        /// </summary>
        public Type RequestExampleType { get; set; }

        /// <summary>
        /// Gets or sets the HTTP response resource example type or null.
        /// </summary>
        public Type ResponseExampleType { get; set; }

        /// <summary>
        /// Gets or sets the result type.
        /// </summary>
        public Type ResultType { get; set; }

        /// <summary>
        /// Gets or sets the service URL.
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets a comma separated list of supported HTTP methods.
        /// </summary>
        public string SupportedHttpMethods { get; set; }

        /// <summary>
        /// Gets or sets the URL template.
        /// </summary>
        public string UrlTempate { get; set; }

        /// <summary>
        /// Gets or sets authentication type and default credentials.
        /// </summary>
        public Tuple<AuthenticationType, string> Credentials { get; set; }

        /// <summary>
        /// Gets or sets a collection of additional headers.
        /// </summary>
        public ICollection<Tuple<string, string>> AdditionalHeaders { get; set; }

        /// <summary>
        /// Gets or sets a collection of route parameters.
        /// </summary>
        public ICollection<ProxyParameter> RouteParameters { get; set; }

        /// <summary>
        /// Gets or sets a collection of possible HTTP status codes.
        /// </summary>
        public ICollection<ProxyStatusCode> StatusCodes { get; set; }

        /// <summary>
        /// Gets a value indicating whether the operation has a response.
        /// </summary>
        public bool HasResponse
        {
            get
            {
                return ResultType != null && ResultType != typeof(void) && ResultType != typeof(EmptyResult) && ResultType != typeof(StatusResult);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the operation supports OData queries.
        /// </summary>
        public bool SupportsOdata
        {
            get
            {
                return ResultType != null && ResultType.IsGenericType && ResultType.GetGenericTypeDefinition() == typeof(IQueryable<>);
            }
        }

        /// <summary>
        /// Gets sample URL parts - the service part and the operation part.
        /// </summary>
        /// <returns>A tuple with sample URL parts.</returns>
        public Tuple<string, string> GenerateSampleUrlParts()
        {
            HttpContext context = HttpContext.Current;

            if (context == null || context.Request.ApplicationPath == null)
            {
                return null;
            }

            string urlTemplate = UrlTempate;

            if (urlTemplate.IndexOf('{') > 0)
            {
                urlTemplate = FillRouteParameters(urlTemplate);
            }

            const char Slash = '/';
            string serviceUrl = context.Request.Url.GetLeftPart(UriPartial.Authority).TrimEnd(Slash);

            if (HttpsPort > 0 && !Regex.IsMatch(serviceUrl, ":[0-9]+"))
            {
                serviceUrl = AddUrlPort(serviceUrl);
            }

            return Tuple.Create(String.Concat(serviceUrl, context.Request.ApplicationPath.TrimEnd(Slash), Slash), urlTemplate);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(ProxyOperation other)
        {
            if (other == null)
            {
                return 1;
            }

            int serviceTypeComparision = String.CompareOrdinal(ServiceUrl, ServiceUrl);

            if (serviceTypeComparision != 0)
            {
                return serviceTypeComparision;
            }

            int urlComparision = String.CompareOrdinal(UrlTempate, other.UrlTempate);

            return urlComparision == 0 ? HttpMethod.CompareTo(other.HttpMethod) : urlComparision;
        }

        private string FillRouteParameters(string urlTemplate)
        {
            var routeParametersWithValues = RouteParameters.Where(p => p.ExampleValue != null);

            foreach (ProxyParameter routeParameter in routeParametersWithValues)
            {
                urlTemplate = Regex.Replace(urlTemplate,
                                            String.Concat(@"\{", routeParameter.Name, @"\}"),
                                            HttpUtility.UrlEncode(Convert.ToString(routeParameter.ExampleValue, CultureInfo.InvariantCulture)),
                                            RegexOptions.IgnoreCase);
            }

            return urlTemplate;
        }

        private string AddUrlPort(string serviceUrl)
        {
            serviceUrl = Regex.Replace(serviceUrl, "http://", "https://", RegexOptions.IgnoreCase);

            if (HttpsPort != 443)
            {
                serviceUrl += String.Format(CultureInfo.InvariantCulture, ":{0}", HttpsPort);
            }

            return serviceUrl;
        }
    }
}
