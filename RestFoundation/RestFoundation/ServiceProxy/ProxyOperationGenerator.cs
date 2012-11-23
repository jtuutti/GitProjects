// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using RestFoundation.Runtime;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a proxy operation generator class.
    /// </summary>
    public static class ProxyOperationGenerator
    {
        private const string UrlSeparator = "/";

        /// <summary>
        /// Gets a proxy operation by the ID.
        /// </summary>
        /// <param name="operationId">The proxy operation ID.</param>
        /// <returns>The proxy operation instance.</returns>
        public static ProxyOperation Get(Guid operationId)
        {
            ServiceMethodMetadata metadata = ServiceMethodRegistry.ServiceMethods.SelectMany(m => m.Value).FirstOrDefault(m => m.ServiceMethodId == operationId);

            if (metadata.MethodInfo == null || metadata.UrlInfo == null)
            {
                return null;
            }

            return GenerateProxyOperation(metadata);
        }

        /// <summary>
        /// Gets all proxy operations associated with the service URL.
        /// </summary>
        /// <returns>A sequence of proxy operation.</returns>
        public static IEnumerable<ProxyOperation> GetAll()
        {
            var operations = new SortedSet<ProxyOperation>();

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

                    ProxyOperation operation = GenerateProxyOperation(metadata);

                    if (operation != null)
                    {
                        operations.Add(operation);
                    }
                }
            }

            return operations;
        }

        private static string GetUrlTemplate(ServiceMethodMetadata metadata)
        {
            return (metadata.ServiceUrl + (metadata.UrlInfo.UrlTemplate.Length > 0 ? UrlSeparator + metadata.UrlInfo.UrlTemplate.TrimStart(UrlSeparator[0]) : UrlSeparator)).Trim(UrlSeparator[0]);
        }

        private static string GetSupportedHttpMethods(ServiceMethodMetadata metadata)
        {
            return String.Join(", ", metadata.UrlInfo.HttpMethods.Where(m => m != metadata.UrlInfo.HttpMethods.First()).Select(m => m.ToString().ToUpperInvariant()));
        }

        private static string GetDescription(MethodInfo method, IProxyMetadata proxyMetadata)
        {
            if (proxyMetadata == null)
            {
                return "No description provided";
            }

            return proxyMetadata.GetDescription(method) ?? "No description provided";
        }

        private static List<StatusCodeMetadata> GetStatusCodes(MethodInfo method, IProxyMetadata proxyMetadata, bool hasResource, bool hasResponse, bool requiresHttps)
        {
            var statusCodes = new List<StatusCodeMetadata>();

            if (proxyMetadata != null)
            {
                statusCodes.AddRange(proxyMetadata.GetResponseStatuses(method));
            }

            if (hasResource)
            {
                statusCodes.Add(new StatusCodeMetadata { Code = HttpStatusCode.BadRequest, Condition = "Resource body is invalid" });
                statusCodes.Add(new StatusCodeMetadata { Code = HttpStatusCode.UnsupportedMediaType, Condition = "Media type is not supported" });
            }

            if (hasResponse)
            {
                statusCodes.Add(new StatusCodeMetadata { Code = HttpStatusCode.NotAcceptable, Condition = "Resulting media type is not accepted by the client" });
            }

            if (!statusCodes.Any(code => code.GetNumericStatusCode() >= 200 && code.GetNumericStatusCode() <= 204))
            {
                statusCodes.Add(new StatusCodeMetadata { Code = HttpStatusCode.OK, Condition = "Operation is successful" });
            }

            if (requiresHttps)
            {
                statusCodes.Add(new StatusCodeMetadata { Code = HttpStatusCode.Forbidden, Condition = "HTTP protocol without SSL is not supported" });
            }

            statusCodes.Sort((code1, code2) => code1.CompareTo(code2));

            return statusCodes;
        }

        private static ICollection<ParameterMetadata> GetParameters(ServiceMethodMetadata metadata, IProxyMetadata proxyMetadata)
        {
            var parameters = new List<ParameterMetadata>(GetQueryStringParameters(metadata, proxyMetadata));
            parameters.AddRange(GetRouteParameters(metadata, proxyMetadata));

            return parameters.Distinct().ToList();
        }

        private static IEnumerable<ParameterMetadata> GetRouteParameters(ServiceMethodMetadata metadata, IProxyMetadata proxyMetadata)
        {
            var routeParameters = new List<ParameterMetadata>();

            foreach (ParameterInfo parameter in metadata.MethodInfo.GetParameters())
            {
                if (metadata.UrlInfo.UrlTemplate.IndexOf(String.Concat("{", parameter.Name, "}"), StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                ParameterMetadata parameterMetadata = proxyMetadata != null ? proxyMetadata.GetParameter(metadata.MethodInfo, parameter.Name, true) : new ParameterMetadata();

                var routeParameter = new ParameterMetadata
                {
                    Name = parameter.Name.ToLowerInvariant(),
                    Type = parameter.ParameterType,
                    IsRouteParameter = true,
                    RegexConstraint = GetParameterConstraint(parameter),
                    ExampleValue = parameterMetadata.ExampleValue,
                    AllowedValues = parameterMetadata.AllowedValues
                };

                routeParameters.Add(routeParameter);
            }

            return routeParameters;
        }

        private static IEnumerable<ParameterMetadata> GetQueryStringParameters(ServiceMethodMetadata metadata, IProxyMetadata proxyMetadata)
        {
            var queryParameters = new List<ParameterMetadata>();

            if (proxyMetadata == null)
            {
                return queryParameters;
            }

            foreach (var queryParameter in proxyMetadata.GetParameters(metadata.MethodInfo, false))
            {
                queryParameters.Add(new ParameterMetadata
                {
                    Name = queryParameter.Name.ToLowerInvariant(),
                    Type = queryParameter.Type,
                    RegexConstraint = queryParameter.RegexConstraint,
                    ExampleValue = queryParameter.ExampleValue,
                    AllowedValues = queryParameter.AllowedValues
                });
            }

            return queryParameters;
        }

        private static Tuple<ResourceExampleMetadata, ResourceExampleMetadata> GetResourceExampleTypes(MethodInfo method, IProxyMetadata proxyMetadata)
        {
            if (proxyMetadata == null)
            {
                return new Tuple<ResourceExampleMetadata, ResourceExampleMetadata>(null, null);
            }

            ResourceExampleMetadata requestResourceExample = proxyMetadata.GetRequestResourceExample(method);
            ResourceExampleMetadata responseResourceExample = proxyMetadata.GetResponseResourceExample(method);

            return Tuple.Create(requestResourceExample, responseResourceExample);
        }

        private static string GetParameterConstraint(ParameterInfo parameter)
        {
            var constraintAttribute = Attribute.GetCustomAttribute(parameter, typeof(ParameterConstraintAttribute), false) as ParameterConstraintAttribute;

            return constraintAttribute != null ? constraintAttribute.Pattern.TrimStart('^').TrimEnd('$') : null;
        }

        private static AuthenticationMetadata GetCredentials(MethodInfo method, IProxyMetadata proxyMetadata)
        {
            return proxyMetadata != null ? proxyMetadata.GetAuthentication(method) : null;
        }

        private static bool HasResource(ServiceMethodMetadata metadata, HttpMethod httpMethod)
        {
            if (httpMethod != HttpMethod.Post && httpMethod != HttpMethod.Put && httpMethod != HttpMethod.Patch)
            {
                return false;
            }

            var methodParameters = metadata.MethodInfo.GetParameters();

            if (methodParameters.Any(p => String.Equals("resource", p.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            return methodParameters.Any(p => Attribute.GetCustomAttributes(p, typeof(ResourceParameterAttribute), false).Length > 0);
        }

        private static ProxyOperation GenerateProxyOperation(ServiceMethodMetadata metadata)
        {
            IProxyMetadata proxyMetadata = ServiceContractTypeRegistry.GetProxyMetadata(metadata.MethodInfo.DeclaringType);

            if (proxyMetadata != null && proxyMetadata.IsHidden(metadata.MethodInfo))
            {
                return null;
            }

            var operation = new ProxyOperation
                            {
                                ServiceUrl = metadata.ServiceUrl,
                                UrlTempate = GetUrlTemplate(metadata),
                                HttpMethod = metadata.UrlInfo.HttpMethods.First(),
                                SupportedHttpMethods = GetSupportedHttpMethods(metadata),
                                MetadataUrl = String.Concat("metadata?oid=", metadata.ServiceMethodId),
                                ProxyUrl = String.Concat("proxy?oid=", metadata.ServiceMethodId),
                                Description = GetDescription(metadata.MethodInfo, proxyMetadata),
                                ResultType = metadata.MethodInfo.ReturnType,
                                RouteParameters = GetParameters(metadata, proxyMetadata),
                                HttpsPort = proxyMetadata != null && proxyMetadata.GetHttps(metadata.MethodInfo) != null ? proxyMetadata.GetHttps(metadata.MethodInfo).Port : 0,
                                IsIPFiltered = proxyMetadata != null && proxyMetadata.IsIPFiltered(metadata.MethodInfo),
                                Credentials = GetCredentials(metadata.MethodInfo, proxyMetadata),
                                AdditionalHeaders = proxyMetadata != null ? proxyMetadata.GetHeaders(metadata.MethodInfo) : new List<HeaderMetadata>()
                            };

            operation.StatusCodes = GetStatusCodes(metadata.MethodInfo, proxyMetadata, operation.HasResource, operation.HasResponse, operation.HttpsPort > 0);
            operation.HasResource = HasResource(metadata, operation.HttpMethod);

            Tuple<ResourceExampleMetadata, ResourceExampleMetadata> resourceExampleTypes = GetResourceExampleTypes(metadata.MethodInfo, proxyMetadata);
            operation.RequestResourceExample = resourceExampleTypes.Item1;
            operation.ResponseResourceExample = resourceExampleTypes.Item2;

            return operation;
        }
    }
}
