﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using RestFoundation.Results;

namespace RestFoundation.ServiceProxy.Helpers
{
    public sealed class ProxyOperation : IComparable<ProxyOperation>
    {
        public string ServiceUrl { get; set; }
        public string UrlTempate { get; set; }
        public string MetadataUrl { get; set; }
        public string ProxyUrl { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public string SupportedHttpMethods { get; set; }
        public string Description { get; set; }
        public bool HasResourceParameter { get; set; }
        public Type ResultType { get; set; }
        public List<ProxyStatusCode> StatusCodes { get; set; }
        public List<ProxyRouteParameter> RouteParameters { get; set; }
        public Type RequestExampleType { get; set; }
        public Type ResponseExampleType { get; set; }

        public bool HasResource
        {
            get
            {
                if (HttpMethod != HttpMethod.Post && HttpMethod != HttpMethod.Put && HttpMethod != HttpMethod.Patch)
                {
                    return false;
                }

                return HasResourceParameter;
            }
        }

        public bool HasResponse
        {
            get
            {
                return ResultType != null && ResultType != typeof(void) && ResultType != typeof(EmptyResult) && ResultType != typeof(StatusResult);
            }
        }

        public string GenerateSampleUrl(bool generateAbsoluteUrl)
        {
            string urlTemplate = UrlTempate;

            if (urlTemplate.IndexOf('{') < 0)
            {
                return GenerateAbsoluteUrl(urlTemplate, generateAbsoluteUrl);
            }

            var routeParametersWithValues = RouteParameters.Where(p => p.ExampleValue != null);

            foreach (ProxyRouteParameter routeParameter in routeParametersWithValues)
            {
                urlTemplate = Regex.Replace(urlTemplate,
                                            String.Concat(@"\{", routeParameter.Name, @"\}"),
                                            HttpUtility.UrlEncode(Convert.ToString(routeParameter.ExampleValue, CultureInfo.InvariantCulture)),
                                            RegexOptions.IgnoreCase);
            }

            return urlTemplate.IndexOf('{') < 0 ? GenerateAbsoluteUrl(urlTemplate, generateAbsoluteUrl) : String.Empty;
        }

        private static string GenerateAbsoluteUrl(string urlTemplate, bool generateAbsoluteUrl)
        {
            if (!generateAbsoluteUrl)
            {
                return urlTemplate;
            }

            HttpContext context = HttpContext.Current;

            return context != null ? String.Concat(context.Request.Url.GetLeftPart(UriPartial.Authority), context.Request.ApplicationPath, "/", urlTemplate) : urlTemplate;
        }

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
    }
}
