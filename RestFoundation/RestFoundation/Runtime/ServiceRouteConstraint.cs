// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;
using RestFoundation.Context;

namespace RestFoundation.Runtime
{
    internal sealed class ServiceRouteConstraint : IRouteConstraint
    {
        private static readonly Regex parameterNamePattern = new Regex(@"\{([a-zA-Z0-9_7]+)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly HashSet<HttpMethod> m_httpMethods;
        private readonly Dictionary<string, RouteParameter> m_parameters;

        public ServiceRouteConstraint(ServiceMethodMetadata metadata)
        {
            m_httpMethods = new HashSet<HttpMethod>(metadata.UrlInfo.HttpMethods);
            m_parameters = GetRouteParameters(metadata);
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            if (routeDirection != RouteDirection.IncomingRequest)
            {
                return true;
            }

            var serviceContractTypeName = values[RouteConstants.ServiceContractType] as string;

            if (serviceContractTypeName == null)
            {
                return false;
            }

            Type serviceContractType = ServiceContractTypeRegistry.GetType(serviceContractTypeName);

            if (serviceContractType == null)
            {
                return false;
            }

            var urlTemplate = values[RouteConstants.UrlTemplate] as string;

            if (urlTemplate == null)
            {
                return false;
            }

            return ValidateHttpMethod(httpContext, serviceContractType, urlTemplate) && ValidateRouteParameters(values);
        }

        private static Dictionary<string, RouteParameter> GetRouteParameters(ServiceMethodMetadata metadata)
        {
            var parameters = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (Match match in parameterNamePattern.Matches(metadata.UrlInfo.UrlTemplate))
            {
                parameters.Add(match.Result("$1"));
            }

            var routeParameters = new Dictionary<string, RouteParameter>(StringComparer.OrdinalIgnoreCase);

            foreach (ParameterInfo methodParameter in metadata.MethodInfo.GetParameters())
            {
                if (!parameters.Contains(methodParameter.Name))
                {
                    continue;
                }

                var constraintAttribute = Attribute.GetCustomAttribute(methodParameter, typeof(ParameterConstraintAttribute), false) as ParameterConstraintAttribute;

                if (constraintAttribute != null)
                {
                    routeParameters.Add(methodParameter.Name, new RouteParameter(methodParameter.Name.ToLowerInvariant(), constraintAttribute.PatternRegex));
                }
                else
                {
                    routeParameters.Add(methodParameter.Name, new RouteParameter(methodParameter.Name.ToLowerInvariant(), null));
                }
            }

            return routeParameters;
        }

        private bool ValidateHttpMethod(HttpContextBase httpContext, Type serviceContractType, string urlTemplate)
        {
            HashSet<HttpMethod> allowedHttpMethods = HttpMethodRegistry.GetHttpMethods(new RouteMetadata(serviceContractType.AssemblyQualifiedName, urlTemplate));
            HttpMethod httpMethod = httpContext.GetOverriddenHttpMethod();

            if (httpMethod != HttpMethod.Options)
            {
                if (!allowedHttpMethods.Contains(httpMethod))
                {
                    httpContext.Items[RouteConstants.RouteMethodConstraintFailed] = true;
                    return false;
                }

                if (!m_httpMethods.Contains(httpMethod))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateRouteParameters(IEnumerable<KeyValuePair<string, object>> values)
        {
            foreach (var value in values)
            {
                if (value.Key.StartsWith("_", StringComparison.Ordinal))
                {
                    continue;
                }

                RouteParameter routeParameter;

                if (!m_parameters.TryGetValue(value.Key, out routeParameter))
                {
                    return false;
                }

                if (routeParameter.Contraint != null &&
                    !routeParameter.Contraint.IsMatch(Convert.ToString(value.Value, CultureInfo.InvariantCulture)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
