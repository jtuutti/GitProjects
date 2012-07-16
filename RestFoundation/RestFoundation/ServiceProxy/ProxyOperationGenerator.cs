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

            if (Attribute.GetCustomAttribute(metadata.MethodInfo, typeof(ProxyHiddenOperationAttribute), false) != null)
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

                if (Attribute.GetCustomAttribute(metadata.MethodInfo, typeof(ProxyHiddenOperationAttribute), false) != null)
                {
                    continue;
                }

                foreach (HttpMethod httpMethod in metadata.UrlInfo.HttpMethods)
                {
                    if (httpMethod == HttpMethod.Head || httpMethod == HttpMethod.Options)
                    {
                        continue;
                    }

                    operations.Add(GenerateProxyOperation(metadata));
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

        private static string GetDescription(MethodInfo method)
        {
            var descriptionAttribute = Attribute.GetCustomAttribute(method, typeof(ProxyOperationDescriptionAttribute), true) as ProxyOperationDescriptionAttribute;

            return descriptionAttribute != null ? descriptionAttribute.Description : "No description provided";
        }

        private static List<ProxyStatusCode> GetStatusCodes(MethodInfo methodInfo, bool hasResource, bool hasResponse, bool requiresHttps)
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

            if (requiresHttps)
            {
                statusCodes.Add(new ProxyStatusCode(HttpStatusCode.Forbidden, "HTTP protocol without SSL is not supported"));
            }

            statusCodes.Sort((code1, code2) => code1.CompareTo(code2));

            return statusCodes;
        }

        private static List<ProxyParameter> GetParameters(ServiceMethodMetadata metadata)
        {
            List<ProxyParameter> parameters = GetRouteParameters(metadata);
            parameters.AddRange(GetQueryParameters(metadata));

            return parameters;
        }

        private static List<ProxyParameter> GetRouteParameters(ServiceMethodMetadata metadata)
        {
            var routeParameters = new List<ProxyParameter>();

            foreach (ParameterInfo parameter in metadata.MethodInfo.GetParameters())
            {
                if (metadata.UrlInfo.UrlTemplate.IndexOf(String.Concat("{", parameter.Name, "}"), StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                var routeParameterAttribute = Attribute.GetCustomAttribute(parameter, typeof(ProxyRouteParameterAttribute), true) as ProxyRouteParameterAttribute;
                ProxyParameter routeParameter;

                if (routeParameterAttribute == null)
                {
                    routeParameter = new ProxyParameter(parameter.Name.ToLowerInvariant(), TypeDescriptor.GetTypeName(parameter.ParameterType), GetParameterConstraint(parameter), true);
                }
                else
                {
                    routeParameter = new ProxyParameter(parameter.Name.ToLowerInvariant(),
                                                        TypeDescriptor.GetTypeName(parameter.ParameterType),
                                                        GetParameterConstraint(parameter),
                                                        routeParameterAttribute.ExampleValue,
                                                        routeParameterAttribute.AllowedValues,
                                                        true);
                }

                routeParameters.Add(routeParameter);
            }

            return routeParameters;
        }

        private static List<ProxyParameter> GetQueryParameters(ServiceMethodMetadata metadata)
        {
            var routeParameters = new List<ProxyParameter>();
            var queryParameterAttributes = metadata.MethodInfo.GetCustomAttributes(typeof(ProxyQueryParameterAttribute), false).Cast<ProxyQueryParameterAttribute>();

            foreach (ProxyQueryParameterAttribute queryParameterAttribute in queryParameterAttributes)
            {
                routeParameters.Add(new ProxyParameter(queryParameterAttribute.Name,
                                                       queryParameterAttribute.ParameterType != null ?
                                                                                TypeDescriptor.GetTypeName(queryParameterAttribute.ParameterType) :
                                                                                TypeDescriptor.GetTypeName(typeof(string)),
                                                       queryParameterAttribute.RegexConstraint,
                                                       queryParameterAttribute.ExampleValue,
                                                       queryParameterAttribute.AllowedValues,
                                                       false));
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

        private static string GetParameterConstraint(ParameterInfo parameter)
        {
            var constraintAttribute = Attribute.GetCustomAttribute(parameter, typeof(ParameterConstraintAttribute), false) as ParameterConstraintAttribute;

            return constraintAttribute != null ? constraintAttribute.Pattern.TrimStart('^').TrimEnd('$') : null;
        }

        private static List<Tuple<string, string>> GetAdditionalHeaders(ServiceMethodMetadata metadata)
        {
            List<ProxyAdditionalHeaderAttribute> methodAttributes = metadata.MethodInfo.GetCustomAttributes(typeof(ProxyAdditionalHeaderAttribute), false).Cast<ProxyAdditionalHeaderAttribute>().ToList();

            if (metadata.MethodInfo.DeclaringType != null)
            {
                IEnumerable<ProxyAdditionalHeaderAttribute> contractAttributes = metadata.MethodInfo.DeclaringType.GetCustomAttributes(typeof(ProxyAdditionalHeaderAttribute), false).Cast<ProxyAdditionalHeaderAttribute>();

                foreach (ProxyAdditionalHeaderAttribute contractAttribute in contractAttributes)
                {
                    if (!methodAttributes.Any(a => String.Equals(contractAttribute.Name, a.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        methodAttributes.Add(contractAttribute);
                    }
                }
            }

            methodAttributes.Sort((a1, a2) => String.Compare(a1.Name, a2.Name, StringComparison.OrdinalIgnoreCase));

            return methodAttributes.Select(a => Tuple.Create(a.Name, a.Value)).ToList();
        }

        private static int GetHttpsPort(ServiceMethodMetadata metadata)
        {
            var httpsOnlyAttribute = Attribute.GetCustomAttribute(metadata.MethodInfo, typeof(ProxyHttpsOnlyAttribute), false) as ProxyHttpsOnlyAttribute;

            if (httpsOnlyAttribute == null && metadata.MethodInfo.DeclaringType != null)
            {
                httpsOnlyAttribute = Attribute.GetCustomAttribute(metadata.MethodInfo.DeclaringType, typeof(ProxyHttpsOnlyAttribute), false) as ProxyHttpsOnlyAttribute;
            }

            if (httpsOnlyAttribute == null || httpsOnlyAttribute.Port <= 0)
            {
                return 0;
            }

            return httpsOnlyAttribute.Port;
        }

        private static bool GetIsIPFiltered(ServiceMethodMetadata metadata)
        {
            var isIPFilteredAttribute = Attribute.GetCustomAttribute(metadata.MethodInfo, typeof(ProxyIPFilteredAttribute), false);

            if (isIPFilteredAttribute == null && metadata.MethodInfo.DeclaringType != null)
            {
                isIPFilteredAttribute = Attribute.GetCustomAttribute(metadata.MethodInfo.DeclaringType, typeof(ProxyIPFilteredAttribute), false);
            }

            return isIPFilteredAttribute != null;
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

            return methodParameters.Any(p => Attribute.GetCustomAttribute(p, typeof(ResourceParameterAttribute), false) != null);
        }

        private static ProxyOperation GenerateProxyOperation(ServiceMethodMetadata metadata)
        {
            var operation = new ProxyOperation
                            {
                                ServiceUrl = metadata.ServiceUrl,
                                UrlTempate = GetUrlTemplate(metadata),
                                HttpMethod = metadata.UrlInfo.HttpMethods.First(),
                                SupportedHttpMethods = GetSupportedHttpMethods(metadata),
                                MetadataUrl = String.Concat("metadata?oid=", metadata.ServiceMethodId),
                                ProxyUrl = String.Concat("proxy?oid=", metadata.ServiceMethodId),
                                Description = GetDescription(metadata.MethodInfo),
                                ResultType = metadata.MethodInfo.ReturnType,
                                RouteParameters = GetParameters(metadata),
                                HttpsPort = GetHttpsPort(metadata),
                                IsIPFiltered = GetIsIPFiltered(metadata),
                                AdditionalHeaders = GetAdditionalHeaders(metadata)
                            };

            operation.StatusCodes = GetStatusCodes(metadata.MethodInfo, operation.HasResource, operation.HasResponse, operation.HttpsPort > 0);
            operation.HasResource = HasResource(metadata, operation.HttpMethod);

            Tuple<Type, Type> resourceExampleTypes = GetResourceExampleTypes(metadata.MethodInfo);
            operation.RequestExampleType = resourceExampleTypes.Item1;
            operation.ResponseExampleType = resourceExampleTypes.Item2;

            return operation;
        }
    }
}
