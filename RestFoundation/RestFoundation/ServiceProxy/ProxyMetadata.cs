using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Xml.Serialization;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a service proxy metadata for a service. This class cannot be instantiated.
    /// </summary>
    /// <typeparam name="TContract">The service contract type.</typeparam>
    public abstract class ProxyMetadata<TContract> : IProxyMetadata
        where TContract : class
    {
        private readonly List<HeaderMetadata> m_serviceHeaders = new List<HeaderMetadata>();

        private readonly HashSet<MethodInfo> m_hiddenOperationSet = new HashSet<MethodInfo>();
        private readonly HashSet<MethodInfo> m_ipFilteredSet = new HashSet<MethodInfo>();

        private readonly Dictionary<MethodInfo, string> m_descriptionDictionary = new Dictionary<MethodInfo, string>();
        private readonly Dictionary<MethodInfo, AuthenticationMetadata> m_authenticationDictionary = new Dictionary<MethodInfo, AuthenticationMetadata>();
        private readonly Dictionary<MethodInfo, HttpsMetadata> m_httpsDictionary = new Dictionary<MethodInfo, HttpsMetadata>();
        private readonly Dictionary<MethodInfo, ResourceBuilderMetadata> m_requestResourceBuilderDictionary = new Dictionary<MethodInfo, ResourceBuilderMetadata>();
        private readonly Dictionary<MethodInfo, ResourceBuilderMetadata> m_responseResourceBuilderDictionary = new Dictionary<MethodInfo, ResourceBuilderMetadata>();

        private readonly Dictionary<MethodInfo, List<HeaderMetadata>> m_headerDictionary = new Dictionary<MethodInfo, List<HeaderMetadata>>();
        private readonly Dictionary<MethodInfo, List<ParameterMetadata>> m_parameterDictionary = new Dictionary<MethodInfo, List<ParameterMetadata>>();
        private readonly Dictionary<MethodInfo, List<StatusCodeMetadata>> m_statusCodeDictionary = new Dictionary<MethodInfo, List<StatusCodeMetadata>>();

        private AuthenticationMetadata m_authentication;
        private HttpsMetadata m_https;
        private bool m_isIPFiltered;

        public bool IsIPFiltered(MethodInfo serviceMethod)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            if (m_isIPFiltered)
            {
                return true;
            }

            return m_ipFilteredSet.Contains(serviceMethod);
        }

        public bool IsHidden(MethodInfo serviceMethod)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            return m_hiddenOperationSet.Contains(serviceMethod);
        }

        public string GetDescription(MethodInfo serviceMethod)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            string description;

            if (!m_descriptionDictionary.TryGetValue(serviceMethod, out description))
            {
                return null;
            }

            return description;
        }

        public AuthenticationMetadata GetAuthentication(MethodInfo serviceMethod)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            AuthenticationMetadata authentication;

            if (!m_authenticationDictionary.TryGetValue(serviceMethod, out authentication))
            {
                return m_authentication;
            }

            return authentication;
        }

        public HttpsMetadata GetHttps(MethodInfo serviceMethod)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            HttpsMetadata https;

            if (!m_httpsDictionary.TryGetValue(serviceMethod, out https))
            {
                return m_https;
            }

            return https;
        }

        public ResourceBuilderMetadata GetRequestResourceBuilder(MethodInfo serviceMethod)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            ResourceBuilderMetadata resourceBuilder;

            if (!m_requestResourceBuilderDictionary.TryGetValue(serviceMethod, out resourceBuilder))
            {
                return null;
            }

            return resourceBuilder;
        }

        public ResourceBuilderMetadata GetResponseResourceBuilder(MethodInfo serviceMethod)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            ResourceBuilderMetadata resourceBuilder;

            if (!m_responseResourceBuilderDictionary.TryGetValue(serviceMethod, out resourceBuilder))
            {
                return null;
            }

            return resourceBuilder;
        }

        public IList<HeaderMetadata> GetHeaders(MethodInfo serviceMethod)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            List<HeaderMetadata> headers, operationHeaders;

            if (!m_headerDictionary.TryGetValue(serviceMethod, out operationHeaders))
            {
                headers = new List<HeaderMetadata>(m_serviceHeaders);
            }
            else
            {
                headers = new List<HeaderMetadata>(operationHeaders.Union(m_serviceHeaders));
            }

            return headers;          
        }

        public IList<ParameterMetadata> GetParameters(MethodInfo serviceMethod)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            List<ParameterMetadata> parameters;

            if (!m_parameterDictionary.TryGetValue(serviceMethod, out parameters))
            {
                return new List<ParameterMetadata>();
            }

            return parameters;
        }

        public IList<StatusCodeMetadata> GetStatusCodes(MethodInfo serviceMethod)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            List<StatusCodeMetadata> statusCodes;

            if (!m_statusCodeDictionary.TryGetValue(serviceMethod, out statusCodes))
            {
                return new List<StatusCodeMetadata>();
            }

            return statusCodes;
        }

        public void MarkServiceIPFiltered()
        {
            m_isIPFiltered = true;
        }

        public void MarkOperationIPFiltered(Expression<Func<TContract, object>> serviceMethod)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            MethodInfo serviceMethodInfo = GetMethodInfo(serviceMethod);
            m_ipFilteredSet.Add(serviceMethodInfo);
        }

        public void MarkOperationHidden(Expression<Func<TContract, object>> serviceMethod)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            MethodInfo serviceMethodInfo = GetMethodInfo(serviceMethod);
            m_hiddenOperationSet.Add(serviceMethodInfo);
        }

        public void SetServiceAuthentication(AuthenticationType type)
        {
            SetServiceAuthentication(type, null, null);
        }

        public void SetServiceAuthentication(AuthenticationType type, string defaultUserName)
        {
            SetServiceAuthentication(type, defaultUserName, null);
        }

        public void SetServiceAuthentication(AuthenticationType type, string defaultUserName, string relativeUrlToMatch)
        {
            m_authentication = new AuthenticationMetadata
            {
                Type = type,
                DefaultUserName = defaultUserName,
                RelativeUrlToMatch = relativeUrlToMatch
            };
        }

        public void SetServiceHeader(string name, string value)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            m_serviceHeaders.Add(new HeaderMetadata
            {
                Name = name,
                Value = value
            });
        }

        public void SetServiceHttps(int port)
        {
            if (port <= 0)
            {
                throw new ArgumentOutOfRangeException("port", "Port must be greater than 0");
            }

            m_https = new HttpsMetadata
            {
                Port = port
            };
        }

        public void SetOperationDescription(Expression<Func<TContract, object>> serviceMethod, string operationDescription)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            if (String.IsNullOrEmpty(operationDescription))
            {
                throw new ArgumentNullException("operationDescription");
            }

            MethodInfo serviceMethodInfo = GetMethodInfo(serviceMethod);
            m_descriptionDictionary[serviceMethodInfo] = operationDescription;
        }

        public void SetOperationAuthentication(Expression<Func<TContract, object>> serviceMethod, AuthenticationType type)
        {
            SetOperationAuthentication(serviceMethod, type, null, null);
        }

        public void SetOperationAuthentication(Expression<Func<TContract, object>> serviceMethod, AuthenticationType type, string defaultUserName)
        {
            SetOperationAuthentication(serviceMethod, type, defaultUserName, null);
        }

        public void SetOperationAuthentication(Expression<Func<TContract, object>> serviceMethod, AuthenticationType type, string defaultUserName, string relativeUrlToMatch)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            MethodInfo serviceMethodInfo = GetMethodInfo(serviceMethod);

            m_authenticationDictionary[serviceMethodInfo] = new AuthenticationMetadata
            {
                Type = type,
                DefaultUserName = defaultUserName,
                RelativeUrlToMatch = relativeUrlToMatch
            };
        }

        public void SetOperationResponseResourceBuilder(Expression<Func<TContract, object>> serviceMethod, object instance)
        {
            SetOperationResponseResourceBuilder(serviceMethod, instance, null);
        }

        public void SetOperationResponseResourceBuilder(Expression<Func<TContract, object>> serviceMethod, object instance, XmlSchemas xmlSchemas)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            MethodInfo serviceMethodInfo = GetMethodInfo(serviceMethod);

            m_responseResourceBuilderDictionary[serviceMethodInfo] = new ResourceBuilderMetadata
            {
                Instance = instance,
                XmlSchemas = xmlSchemas
            };
        }

        public void SetOperationHttps(Expression<Func<TContract, object>> serviceMethod)
        {
            SetOperationHttps(serviceMethod, 443);
        }

        public void SetOperationHttps(Expression<Func<TContract, object>> serviceMethod, int port)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            if (port <= 0)
            {
                throw new ArgumentOutOfRangeException("port", "Port must be greater than 0");
            }

            MethodInfo serviceMethodInfo = GetMethodInfo(serviceMethod);

            m_httpsDictionary[serviceMethodInfo] = new HttpsMetadata
            {
                Port = port
            };
        }

        public void SetOperationHeader(Expression<Func<TContract, object>> serviceMethod, string name, string value)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            MethodInfo serviceMethodInfo = GetMethodInfo(serviceMethod);
            List<HeaderMetadata> headers;

            if (!m_headerDictionary.TryGetValue(serviceMethodInfo, out headers))
            {
                headers = new List<HeaderMetadata>();
            }

            headers.Add(new HeaderMetadata
            {
                Name = name,
                Value = value
            });

            m_headerDictionary[serviceMethodInfo] = headers;
        }

        public void SetOperationParameter(Expression<Func<TContract, object>> serviceMethod, string name, ParameterType type)
        {
            SetOperationParameter(serviceMethod, name, type, null, null, null);
        }

        public void SetOperationParameter(Expression<Func<TContract, object>> serviceMethod, string name, ParameterType type, object exampleValue)
        {
            SetOperationParameter(serviceMethod, name, type, exampleValue, null, null);
        }

        public void SetOperationParameter(Expression<Func<TContract, object>> serviceMethod, string name, ParameterType type, object exampleValue, IList<object> allowedValues)
        {
            SetOperationParameter(serviceMethod, name, type, exampleValue, allowedValues, null);
        }

        public void SetOperationParameter(Expression<Func<TContract, object>> serviceMethod, string name, ParameterType type, object exampleValue, IList<object> allowedValues, string regexConstraint)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            MethodInfo serviceMethodInfo = GetMethodInfo(serviceMethod);
            List<ParameterMetadata> parameters;

            if (!m_parameterDictionary.TryGetValue(serviceMethodInfo, out parameters))
            {
                parameters = new List<ParameterMetadata>();
            }

            parameters.Add(new ParameterMetadata
            {
                Name = name,
                Type = type,
                ExampleValue = exampleValue,
                AllowedValues = allowedValues,
                RegexConstraint = regexConstraint
            });

            m_parameterDictionary[serviceMethodInfo] = parameters;
        }

        public void SetOperationRequestResourceBuilder(Expression<Func<TContract, object>> serviceMethod, object instance)
        {
            SetOperationRequestResourceBuilder(serviceMethod, instance, null);
        }

        public void SetOperationRequestResourceBuilder(Expression<Func<TContract, object>> serviceMethod, object instance, XmlSchemas xmlSchemas)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            MethodInfo serviceMethodInfo = GetMethodInfo(serviceMethod);

            m_requestResourceBuilderDictionary[serviceMethodInfo] = new ResourceBuilderMetadata
            {
                Instance = instance,
                XmlSchemas = xmlSchemas
            };
        }

        public void SetOperationStatusCode(Expression<Func<TContract, object>> serviceMethod, HttpStatusCode statusCode)
        {
            SetOperationStatusCode(serviceMethod, statusCode, null);
        }

        public void SetOperationStatusCode(Expression<Func<TContract, object>> serviceMethod, HttpStatusCode statusCode, string statusDescription)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            MethodInfo serviceMethodInfo = GetMethodInfo(serviceMethod);
            List<StatusCodeMetadata> statusCodes;

            if (!m_statusCodeDictionary.TryGetValue(serviceMethodInfo, out statusCodes))
            {
                statusCodes = new List<StatusCodeMetadata>();
            }

            statusCodes.Add(new StatusCodeMetadata
            {
                StatusCode = statusCode,
                StatusDescription = statusDescription
            });

            m_statusCodeDictionary[serviceMethodInfo] = statusCodes;
        }

        public abstract void Initialize();

        private static MethodInfo GetMethodInfo(Expression<Func<TContract, object>> serviceMethod)
        {
            var methodExpression = serviceMethod.Body as MethodCallExpression;

            if (methodExpression == null)
            {
                throw new ArgumentException("Invalid service method expression provided.", "serviceMethod");
            }

            return methodExpression.Method;
        }
    }
}
