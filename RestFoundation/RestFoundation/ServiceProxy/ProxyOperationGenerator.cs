using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using RestFoundation.Runtime;

namespace RestFoundation.ServiceProxy
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

            if (Attribute.GetCustomAttribute(metadata.MethodInfo, typeof(ProxyHiddenOperation), false) != null)
            {
                return null;
            }

            return GenerateProxyOperation(metadata);
        }

        public static IEnumerable<ProxyOperation> Generate()
        {
            var operations = new SortedSet<ProxyOperation>();

            foreach (ServiceMethodMetadata metadata in ServiceMethodRegistry.ServiceMethods.SelectMany(m => m.Value))
            {
                if (metadata.UrlInfo == null || metadata.UrlInfo.UrlTemplate == null)
                {
                    continue;
                }

                if (Attribute.GetCustomAttribute(metadata.MethodInfo, typeof(ProxyHiddenOperation), false) != null)
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
                    routeParameter = new ProxyParameter(parameter.Name.ToLowerInvariant(), GetParameterType(parameter.ParameterType), GetParameterConstraint(parameter), true);
                }
                else
                {
                    routeParameter = new ProxyParameter(parameter.Name.ToLowerInvariant(),
                                                        GetParameterType(parameter.ParameterType),
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
                                                       queryParameterAttribute.ParameterType != null ? GetParameterType(queryParameterAttribute.ParameterType) : GetParameterType(typeof(string)),
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

        private static string GetParameterType(Type parameterType)
        {
            string type;

            if (parameterType == typeof(string))
            {
                type = "string";
            }
            else if (parameterType == typeof(char) || parameterType == typeof(char?))
            {
                type = "char";
            }
            else if (parameterType == typeof(bool) || parameterType == typeof(bool?))
            {
                type = "boolean";
            }
            else if (parameterType == typeof(Decimal) || parameterType == typeof(Decimal?))
            {
                type = "decimal";
            }
            else if (parameterType == typeof(float) || parameterType == typeof(float?))
            {
                type = "float";
            }
            else if (parameterType == typeof(double) || parameterType == typeof(double?))
            {
                type = "double";
            }
            else if (parameterType == typeof(DateTime) || parameterType == typeof(DateTime?))
            {
                type = "datetime";
            }
            else if (parameterType == typeof(long) || parameterType == typeof(long?))
            {
                type = "long";
            }
            else if (parameterType == typeof(int) || parameterType == typeof(int?))
            {
                type = "int";
            }
            else if (parameterType == typeof(short) || parameterType == typeof(short?))
            {
                type = "short";
            }
            else if (parameterType == typeof(byte) || parameterType == typeof(byte?))
            {
                type = "byte";
            }
            else if (parameterType == typeof(ulong) || parameterType == typeof(ulong?))
            {
                type = "unsignedLong";
            }
            else if (parameterType == typeof(uint) || parameterType == typeof(uint?))
            {
                type = "unsignedInt";
            }
            else if (parameterType == typeof(ushort) || parameterType == typeof(ushort?))
            {
                type = "unsignedShort";
            }
            else if (parameterType == typeof(Guid) || parameterType == typeof(Guid?))
            {
                type = "guid";
            }
            else if (parameterType == typeof(Uri))
            {
                type = "anyURI";
            }
            else
            {
                type = "object";
            }

            return type;
        }

        private static string GetParameterConstraint(ParameterInfo parameter)
        {
            var constraintAttribute = Attribute.GetCustomAttribute(parameter, typeof(ParameterConstraintAttribute), false) as ParameterConstraintAttribute;

            return constraintAttribute != null ? constraintAttribute.Pattern.ToString().TrimStart('^').TrimEnd('$') : null;
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

            return methodParameters.Any(p => Attribute.GetCustomAttribute(p, typeof(BindResourceAttribute), false) != null);
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
                                IsIpFiltered = (metadata.Acl != null),
                                AdditionalHeaders = GetAdditionalHeaders(metadata)
                            };

            operation.StatusCodes = GetStatusCodes(metadata.MethodInfo, operation.HasResource, operation.HasResponse);
            operation.HasResource = HasResource(metadata, operation.HttpMethod);

            Tuple<Type, Type> resourceExampleTypes = GetResourceExampleTypes(metadata.MethodInfo);
            operation.RequestExampleType = resourceExampleTypes.Item1;
            operation.ResponseExampleType = resourceExampleTypes.Item2;

            return operation;
        }
    }
}
