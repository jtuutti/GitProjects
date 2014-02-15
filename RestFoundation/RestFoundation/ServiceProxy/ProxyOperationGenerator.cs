// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using RestFoundation.Runtime;
using RestFoundation.ServiceProxy.OperationMetadata;
using RestFoundation.TypeBinders;

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
        public static ProxyOperation Get(int operationId)
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

                if (metadata.MethodInfo.DeclaringType.GetCustomAttribute<ProxyHiddenAttribute>() != null)
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

        private static string GetUrlTemplate(ServiceMethodMetadata metadata, IProxyMetadata proxyMetadata)
        {
            string urlTemplate = (metadata.ServiceUrl + (metadata.UrlInfo.UrlTemplate.Length > 0 ? UrlSeparator + metadata.UrlInfo.UrlTemplate.TrimStart(UrlSeparator[0]) : UrlSeparator)).Trim(UrlSeparator[0]);

            if (proxyMetadata == null)
            {
                return urlTemplate;
            }

            var parameters = GetQueryStringParameters(metadata, proxyMetadata).Where(p => p.ExampleValue != null);
            var parameterBuilder = new StringBuilder();

            foreach (var parameter in parameters)
            {
                if (parameterBuilder.Length > 0)
                {
                    parameterBuilder.Append("&");
                }

                parameterBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}={{{0}}}", HttpUtility.UrlEncode(parameter.Name));
            }

            if (parameterBuilder.Length == 0)
            {
                return urlTemplate;
            }

            return urlTemplate + "?" + parameterBuilder;
        }

        private static string GetSupportedHttpMethods(ServiceMethodMetadata metadata)
        {
            return String.Join(", ", metadata.UrlInfo.HttpMethods.Where(m => m != metadata.UrlInfo.HttpMethods.First()).Select(m => m.ToString().ToUpperInvariant()));
        }

        private static string GetDescription(MethodInfo method, IProxyMetadata proxyMetadata, XmlDocMetadata methodDocumentation)
        {
            if (proxyMetadata == null)
            {
                return Resources.Global.MissingDescription;
            }

            string description = proxyMetadata.GetDescription(method);

            if (description == null && methodDocumentation != null)
            {
                description = methodDocumentation.Summary;
            }

            return (description ?? Resources.Global.MissingDescription).Trim();
        }

        private static string GetLongDescription(MethodInfo method, IProxyMetadata proxyMetadata, XmlDocMetadata methodDocumentation)
        {
            if (proxyMetadata == null)
            {
                return Resources.Global.MissingDescription;
            }

            string description = proxyMetadata.GetLongDescription(method);

            if (description == null && methodDocumentation != null)
            {
                description = methodDocumentation.Remarks;
            }

            return (description ?? Resources.Global.MissingDescription).Trim();
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
                statusCodes.Add(new StatusCodeMetadata { Code = HttpStatusCode.BadRequest, Condition = Resources.Global.InvalidResourceBody });
                statusCodes.Add(new StatusCodeMetadata { Code = HttpStatusCode.UnsupportedMediaType, Condition = Resources.Global.UnsupportedMediaType });
            }

            if (hasResponse)
            {
                statusCodes.Add(new StatusCodeMetadata { Code = HttpStatusCode.NotAcceptable, Condition = Resources.Global.NonAcceptedMediaType });
            }

            if (!statusCodes.Any(code => code.GetNumericStatusCode() >= 200 && code.GetNumericStatusCode() <= 204))
            {
                statusCodes.Add(new StatusCodeMetadata { Code = HttpStatusCode.OK, Condition = Resources.Global.SuccessfulOperation });
            }

            if (requiresHttps)
            {
                statusCodes.Add(new StatusCodeMetadata { Code = HttpStatusCode.Forbidden, Condition = Resources.Global.HttpsRequiredStatusDescription });
            }

            statusCodes.Sort((code1, code2) => code1.CompareTo(code2));

            return statusCodes;
        }

        private static ICollection<ParameterMetadata> GetParameters(ServiceMethodMetadata metadata, IProxyMetadata proxyMetadata)
        {
            var parameters = new List<ParameterMetadata>(GetQueryStringParameters(metadata, proxyMetadata));
            parameters.AddRange(GetBodyParameters(metadata, proxyMetadata));
            parameters.AddRange(GetRouteParameters(metadata, proxyMetadata));

            IReadOnlyList<XmlDocMetadata> contractDocumentation;
            XmlDocMetadata methodDocumentation = null;

            var xmlDocFactory = Rest.Configuration.Options.XmlDocFactory;

            if (xmlDocFactory != null && metadata.MethodInfo.DeclaringType != null &&
                xmlDocFactory.Value.TryGetValue(metadata.MethodInfo.DeclaringType, out contractDocumentation))
            {
                methodDocumentation = contractDocumentation.FirstOrDefault(x => x.Method == metadata.MethodInfo);
            }

            if (methodDocumentation != null)
            {
                foreach (ParameterMetadata parameter in parameters)
                {
                    string description;

                    if (methodDocumentation.Parameters.TryGetValue(parameter.Name, out description))
                    {
                        parameter.Description = description;
                    }
                }
            }

            return parameters.Distinct().ToList();
        }

        private static IEnumerable<ParameterMetadata> GetQueryStringParameters(ServiceMethodMetadata metadata, IProxyMetadata proxyMetadata)
        {
            var parameters = new List<ParameterMetadata>();

            if (proxyMetadata == null)
            {
                return parameters;
            }

            foreach (var parameter in proxyMetadata.GetParameters(metadata.MethodInfo, RequestParameterType.Query))
            {
                parameters.Add(new ParameterMetadata
                {
                    Name = parameter.Name.ToLowerInvariant(),
                    ParameterType = parameter.ParameterType,
                    Type = parameter.Type,
                    IsOptionalParameter = parameter.IsOptionalParameter,
                    RegexConstraint = parameter.RegexConstraint,
                    ExampleValue = parameter.ExampleValue,
                    AllowedValues = parameter.AllowedValues
                });
            }

            var queryParameters = metadata.MethodInfo.GetParameters()
                                                     .Where(x => Attribute.IsDefined(x, typeof(FromUriAttribute)) &&
                                                                 parameters.All(p => p.Name != x.Name.ToLowerInvariant()))
                                                     .ToArray();

            foreach (ParameterInfo queryParameter in queryParameters)
            {
                parameters.Add(new ParameterMetadata
                {
                    Name = queryParameter.Name.ToLowerInvariant(),
                    ParameterType = RequestParameterType.Query,
                    Type = queryParameter.ParameterType,
                    IsOptionalParameter = queryParameter.DefaultValue != DBNull.Value
                });
            }

            return parameters;
        }

        private static IEnumerable<ParameterMetadata> GetBodyParameters(ServiceMethodMetadata metadata, IProxyMetadata proxyMetadata)
        {
            var parameters = new List<ParameterMetadata>();

            if (proxyMetadata == null)
            {
                return parameters;
            }

            foreach (var parameter in proxyMetadata.GetParameters(metadata.MethodInfo, RequestParameterType.Body))
            {
                parameters.Add(new ParameterMetadata
                {
                    Name = parameter.Name.ToLowerInvariant(),
                    ParameterType = parameter.ParameterType,
                    Type = parameter.Type,
                    IsOptionalParameter = parameter.IsOptionalParameter,
                    RegexConstraint = parameter.RegexConstraint,
                    ExampleValue = parameter.ExampleValue,
                    AllowedValues = parameter.AllowedValues
                });
            }

            var bodyParameters = metadata.MethodInfo.GetParameters()
                                                    .Where(x => Attribute.IsDefined(x, typeof(FromBodyAttribute)) &&
                                                                parameters.All(p => p.Name != x.Name.ToLowerInvariant()))
                                                    .ToArray();

            foreach (ParameterInfo bodyParameter in bodyParameters)
            {
                parameters.Add(new ParameterMetadata
                {
                    Name = bodyParameter.Name.ToLowerInvariant(),
                    ParameterType = RequestParameterType.Body,
                    Type = bodyParameter.ParameterType
                });
            }

            return parameters;
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

                ParameterMetadata parameterMetadata = proxyMetadata != null ?
                                                                proxyMetadata.GetParameter(metadata.MethodInfo, parameter.Name, RequestParameterType.Route) :
                                                                null;

                var routeParameter = new ParameterMetadata
                {
                    Name = parameter.Name.ToLowerInvariant(),
                    ParameterType = RequestParameterType.Route,
                    Type = parameter.ParameterType,
                    IsOptionalParameter = parameter.DefaultValue != DBNull.Value,
                    RegexConstraint = GetParameterConstraint(parameter),
                    ExampleValue = parameterMetadata != null ? parameterMetadata.ExampleValue : null,
                    AllowedValues = parameterMetadata != null ? parameterMetadata.AllowedValues : null
                };

                routeParameters.Add(routeParameter);
            }

            return routeParameters;
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
            var constraintAttribute = parameter.GetCustomAttribute<ConstraintAttribute>(false);

            return constraintAttribute != null ? constraintAttribute.Pattern.TrimStart('^').TrimEnd('$') : null;
        }

        private static AuthenticationMetadata GetCredentials(ServiceMethodMetadata metadata, IProxyMetadata proxyMetadata)
        {
            if (proxyMetadata == null)
            {
                return null;
            }

            AuthenticationMetadata authenticationInfo = proxyMetadata.GetAuthentication(metadata.MethodInfo);

            if (authenticationInfo == null)
            {
                return null;
            }

            if (!String.IsNullOrEmpty(authenticationInfo.RelativeUrlToMatch) &&
                !authenticationInfo.RelativeUrlToMatch.Trim().TrimStart('~', '/').Equals(metadata.ServiceUrl, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return authenticationInfo;
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

            return methodParameters.Any(p => Attribute.IsDefined(p, typeof(ResourceAttribute), false));
        }

        private static ProxyOperation GenerateProxyOperation(ServiceMethodMetadata metadata)
        {
            if (metadata.MethodInfo.DeclaringType == null)
            {
                return null;
            }

            IProxyMetadata proxyMetadata = ServiceContractTypeRegistry.GetProxyMetadata(metadata.MethodInfo.DeclaringType);

            if (proxyMetadata != null && proxyMetadata.IsHidden(metadata.MethodInfo))
            {
                return null;
            }

            IReadOnlyList<XmlDocMetadata> contractDocumentation;
            XmlDocMetadata methodDocumentation = null;

            var xmlDocFactory = Rest.Configuration.Options.XmlDocFactory;

            if (xmlDocFactory != null && xmlDocFactory.Value.TryGetValue(metadata.MethodInfo.DeclaringType, out contractDocumentation))
            {
                methodDocumentation = contractDocumentation.FirstOrDefault(x => x.Method == metadata.MethodInfo);
            }

            var operation = new ProxyOperation
            {
                ServiceUrl = metadata.ServiceUrl,
                UrlTempate = GetUrlTemplate(metadata, proxyMetadata),
                HttpMethod = metadata.UrlInfo.HttpMethods.First(),
                SupportedHttpMethods = GetSupportedHttpMethods(metadata),
                MetadataUrl = String.Concat("metadata?oid=", metadata.ServiceMethodId),
                ProxyUrl = String.Concat("proxy?oid=", metadata.ServiceMethodId),
                Description = GetDescription(metadata.MethodInfo, proxyMetadata, methodDocumentation),
                LongDescription = GetLongDescription(metadata.MethodInfo, proxyMetadata, methodDocumentation),
                ResultType = metadata.MethodInfo.ReturnType,
                RouteParameters = GetParameters(metadata, proxyMetadata),
                HttpsPort = proxyMetadata != null && proxyMetadata.GetHttps(metadata.MethodInfo) != null ? proxyMetadata.GetHttps(metadata.MethodInfo).Port : 0,
                IsIPFiltered = proxyMetadata != null && proxyMetadata.IsIPFiltered(metadata.MethodInfo),
                DoesNotSupportJson = proxyMetadata != null && !proxyMetadata.HasJsonSupport(metadata.MethodInfo),
                DoesNotSupportXml = proxyMetadata != null && !proxyMetadata.HasXmlSupport(metadata.MethodInfo),
                Credentials = GetCredentials(metadata, proxyMetadata),
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
