using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using RestFoundation.Runtime;
using RestFoundation.ServiceProxy.Attributes;

namespace RestFoundation.ServiceProxy.Helpers
{
    public static class ProxyOperationGenerator
    {
        private const string UrlSeparator = "/";

        public static ProxyOperation Get(Guid operationId)
        {
            ServiceMethodMetadata metadata = ServiceMethodRegistry.ServiceMethods.SelectMany(m => m.Value).FirstOrDefault(m => m.ServiceMethodId == operationId);

            if (metadata.MethodInfo == null || metadata.UrlInfo == null)
            {
                return null;
            }

            var operation = new ProxyOperation
                            {
                                ServiceUrl = metadata.ServiceUrl,
                                UrlTempate = GetUrlTemplate(metadata),
                                HttpMethod = metadata.UrlInfo.HttpMethods.First(),
                                SupportedHttpMethods = GetSupportedHttpMethods(metadata),
                                Description = GetDescription(metadata.MethodInfo),
                                HasResourceParameter = metadata.MethodInfo.GetParameters().Any(p => String.Equals("resource", p.Name, StringComparison.OrdinalIgnoreCase)),
                                ResultType = metadata.MethodInfo.ReturnType,
                                RouteParameters = GetRouteParameters(metadata)
                            };

            operation.StatusCodes = GetStatusCodes(metadata.MethodInfo, operation.HasResource, operation.HasResponse);

            Tuple<Type, Type> resourceExampleTypes = GetResourceExampleTypes(metadata.MethodInfo);
            operation.RequestExampleType = resourceExampleTypes.Item1;
            operation.ResponseExampleType = resourceExampleTypes.Item2;

            return operation;
        }

        public static IEnumerable<ProxyOperation> Generate()
        {
            var endPoints = new SortedSet<ProxyOperation>();

            foreach (ServiceMethodMetadata metadata in ServiceMethodRegistry.ServiceMethods.SelectMany(m => m.Value))
            {
                if (metadata.UrlInfo == null || metadata.UrlInfo.UrlTemplate == null)
                {
                    continue;
                }

                foreach (HttpMethod httpMethod in metadata.UrlInfo.HttpMethods)
                {
                    if (httpMethod == HttpMethod.Head || httpMethod == HttpMethod.Options)
                    {
                        continue;
                    }

                    endPoints.Add(new ProxyOperation
                                  {
                                      ServiceUrl = metadata.ServiceUrl,
                                      UrlTempate = GetUrlTemplate(metadata),
                                      HttpMethod = httpMethod,
                                      MetadataUrl = String.Concat("metadata.aspx?oid=", metadata.ServiceMethodId),
                                      ProxyUrl = String.Concat("proxy.aspx?oid=", metadata.ServiceMethodId),
                                      Description = GetDescription(metadata.MethodInfo)
                                  });
                }
            }

            return endPoints;
        }

        private static string GetUrlTemplate(ServiceMethodMetadata metadata)
        {
            return (metadata.ServiceUrl + (metadata.UrlInfo.UrlTemplate.Length > 0 ? UrlSeparator + metadata.UrlInfo.UrlTemplate.TrimStart(UrlSeparator[0]) : UrlSeparator)).Trim(UrlSeparator[0]);
        }

        private static string GetSupportedHttpMethods(ServiceMethodMetadata metadata)
        {
            return String.Join(", ", metadata.UrlInfo.HttpMethods.Where(m => m != metadata.UrlInfo.HttpMethods.First()).Select(m => m.ToString().ToUpperInvariant()));
        }

        private static string GetDescription(MethodInfo method)
        {
            var descriptionAttribute = Attribute.GetCustomAttribute(method, typeof(ProxyOperationDescriptionAttribute), true) as ProxyOperationDescriptionAttribute;

            return descriptionAttribute != null ? descriptionAttribute.Description : "No description provided";
        }

        private static List<ProxyStatusCode> GetStatusCodes(MethodInfo methodInfo, bool hasResource, bool hasResponse)
        {
            var statusAttributes = methodInfo.GetCustomAttributes(typeof(ProxyStatusCodeAttribute), true).Cast<ProxyStatusCodeAttribute>();
            var statusCodes = new List<ProxyStatusCode>();

            foreach (ProxyStatusCodeAttribute statusAttribute in statusAttributes)
            {
                statusCodes.Add(new ProxyStatusCode(statusAttribute.StatusCode, statusAttribute.Condition));
            }

            if (hasResource)
            {
                statusCodes.Add(new ProxyStatusCode(HttpStatusCode.BadRequest, "Resource body is invalid"));
                statusCodes.Add(new ProxyStatusCode(HttpStatusCode.UnsupportedMediaType, "Content type is not supported"));
            }

            if (hasResponse)
            {
                statusCodes.Add(new ProxyStatusCode(HttpStatusCode.NotAcceptable, "Resulting content type is not accepted by the client"));
            }

            if (!statusCodes.Any(code => code.GetNumericStatusCode() >= 200 && code.GetNumericStatusCode() <= 204))
            {
                statusCodes.Add(new ProxyStatusCode(HttpStatusCode.OK, "Operation is successful"));
            }

            statusCodes.Sort((code1, code2) => code1.CompareTo(code2));

            return statusCodes;
        }

        private static List<ProxyRouteParameter> GetRouteParameters(ServiceMethodMetadata metadata)
        {
            var routeParameters = new List<ProxyRouteParameter>();

            foreach (ParameterInfo parameter in metadata.MethodInfo.GetParameters())
            {
                if (metadata.UrlInfo.UrlTemplate.IndexOf(String.Concat("{", parameter.Name, "}"), StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                var routeParameterAttribute = Attribute.GetCustomAttribute(parameter, typeof(ProxyRouteParameterAttribute), true) as ProxyRouteParameterAttribute;
                ProxyRouteParameter routeParameter;

                if (routeParameterAttribute == null)
                {
                    routeParameter = new ProxyRouteParameter(parameter.Name.ToLowerInvariant(), "string");
                }
                else
                {
                    routeParameter = new ProxyRouteParameter(parameter.Name.ToLowerInvariant(), "string", routeParameterAttribute.ExampleValue, routeParameterAttribute.AllowedValues);
                }

                routeParameters.Add(routeParameter);
            }

            return routeParameters;
        }

        private static Tuple<Type, Type> GetResourceExampleTypes(MethodInfo methodInfo)
        {
            var resourceExampleAttribute = Attribute.GetCustomAttribute(methodInfo, typeof(ProxyResourceExampleAttribute), false) as ProxyResourceExampleAttribute;

            if (resourceExampleAttribute == null)
            {
                return new Tuple<Type, Type>(null, null);
            }

            return Tuple.Create(resourceExampleAttribute.RequestExampleType, resourceExampleAttribute.ResponseExampleType);
        }
    }
}
