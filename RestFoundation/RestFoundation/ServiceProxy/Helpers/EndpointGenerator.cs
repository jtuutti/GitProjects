using System;
using System.Collections.Generic;
using System.Linq;
using RestFoundation.Runtime;
using RestFoundation.ServiceProxy.Attributes;

namespace RestFoundation.ServiceProxy.Helpers
{
    public static class EndPointGenerator
    {
        private const string UrlSeparator = "/";

        public static IEnumerable<EndPoint> Generate()
        {
            var endPoints = new SortedSet<EndPoint>();

            foreach (ActionMethodMetadata metadata in ActionMethodRegistry.ActionMethods.SelectMany(m => m.Value))
            {
                if (metadata.UrlInfo == null || metadata.UrlInfo.UrlTemplate == null)
                {
                    continue;
                }

                var descriptionAttribute = Attribute.GetCustomAttribute(metadata.MethodInfo, typeof(UrlMetadataAttribute)) as UrlMetadataAttribute;
                string description = descriptionAttribute != null ? descriptionAttribute.Description : "No description provided";

                foreach (HttpMethod httpMethod in metadata.UrlInfo.HttpMethods)
                {
                    if (httpMethod == HttpMethod.Head || httpMethod == HttpMethod.Options)
                    {
                        continue;
                    }

                    endPoints.Add(new EndPoint
                                  {
                                      ServiceUrl = metadata.ServiceUrl,
                                      UrlTempate = GetUrlTemplate(metadata),
                                      HttpMethod = httpMethod,
                                      RelativeUrl = GetRelativeUrl(metadata.ServiceUrl, metadata.UrlInfo.UrlTemplate, descriptionAttribute != null ? descriptionAttribute.RelativeUrl : null),
                                      Description = description
                                  });
                }
            }

            return endPoints;
        }

        private static string GetUrlTemplate(ActionMethodMetadata metadata)
        {
            return (metadata.ServiceUrl + (metadata.UrlInfo.UrlTemplate.Length > 0 ? UrlSeparator + metadata.UrlInfo.UrlTemplate.TrimStart(UrlSeparator[0]) : UrlSeparator)).Trim(UrlSeparator[0]);
        }

        public static string GetRelativeUrl(string serviceUrl, string urlTemplate, string relativeUrl)
        {
            string endpointUrl = String.Empty;

            if (!String.IsNullOrWhiteSpace(relativeUrl))
            {
                endpointUrl = relativeUrl;
            }
            else if (urlTemplate.Length > 0)
            {
                if (urlTemplate.IndexOf('{') >= 0)
                {
                    return "#";
                }

                endpointUrl = urlTemplate;
            }

            return String.Concat(serviceUrl, UrlSeparator, endpointUrl).Trim(UrlSeparator[0]);
        }
    }
}
