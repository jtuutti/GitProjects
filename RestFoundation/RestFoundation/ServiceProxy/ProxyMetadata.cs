using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Xml.Serialization;
using RestFoundation.Runtime;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a service proxy metadata for a service. This class cannot be instantiated.
    /// </summary>
    /// <typeparam name="TContract">The service contract type.</typeparam>
    public abstract class ProxyMetadata<TContract> : IMethodMetadata, IProxyMetadata
        where TContract : class
    {
        private readonly List<HeaderMetadata> m_serviceHeaders = new List<HeaderMetadata>();

        private readonly HashSet<MethodInfo> m_hiddenOperationSet = new HashSet<MethodInfo>();
        private readonly HashSet<MethodInfo> m_ipFilteredSet = new HashSet<MethodInfo>();

        private readonly Dictionary<MethodInfo, string> m_descriptionDictionary = new Dictionary<MethodInfo, string>();
        private readonly Dictionary<MethodInfo, AuthenticationMetadata> m_authenticationDictionary = new Dictionary<MethodInfo, AuthenticationMetadata>();
        private readonly Dictionary<MethodInfo, HttpsMetadata> m_httpsDictionary = new Dictionary<MethodInfo, HttpsMetadata>();
        private readonly Dictionary<MethodInfo, ResourceExampleMetadata> m_requestResourceExampleDictionary = new Dictionary<MethodInfo, ResourceExampleMetadata>();
        private readonly Dictionary<MethodInfo, ResourceExampleMetadata> m_responseResourceExampleDictionary = new Dictionary<MethodInfo, ResourceExampleMetadata>();

        private readonly Dictionary<MethodInfo, List<HeaderMetadata>> m_headerDictionary = new Dictionary<MethodInfo, List<HeaderMetadata>>();
        private readonly Dictionary<MethodInfo, List<ParameterMetadata>> m_parameterDictionary = new Dictionary<MethodInfo, List<ParameterMetadata>>();
        private readonly Dictionary<MethodInfo, List<StatusCodeMetadata>> m_statusCodeDictionary = new Dictionary<MethodInfo, List<StatusCodeMetadata>>();

        private AuthenticationMetadata m_authentication;
        private HttpsMetadata m_https;
        private bool m_isIPFiltered;

        private MethodInfo m_currentServiceMethod;

        public void SetAuthentication(AuthenticationType type)
        {
            SetAuthentication(type, null, null);
        }

        public void SetAuthentication(AuthenticationType type, string defaultUserName)
        {
            SetAuthentication(type, defaultUserName, null);
        }

        public void SetAuthentication(AuthenticationType type, string defaultUserName, string relativeUrlToMatch)
        {
            m_authentication = new AuthenticationMetadata
            {
                Type = type,
                DefaultUserName = defaultUserName,
                RelativeUrlToMatch = relativeUrlToMatch
            };
        }

        public void SetHeader(string name, string value)
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

        public void SetIPFiltered()
        {
            m_isIPFiltered = true;
        }

        public void SetHttps(int port)
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
        
        public IMethodMetadata ForMethod(Expression<Func<TContract, object>> serviceMethod)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            var methodExpression = serviceMethod.Body as MethodCallExpression;

            if (methodExpression == null || methodExpression.Method == null)
            {
                throw new ArgumentException("Invalid service method expression provided.", "serviceMethod");
            }

            m_currentServiceMethod = methodExpression.Method;

            return this;
        }

        public abstract void Initialize();

        IMethodMetadata IMethodMetadata.SetAuthentication(AuthenticationType type)
        {
            return ((IMethodMetadata) this).SetAuthentication(type, null, null);
        }

        IMethodMetadata IMethodMetadata.SetAuthentication(AuthenticationType type, string defaultUserName)
        {
            return ((IMethodMetadata) this).SetAuthentication(type, defaultUserName, null);
        }

        IMethodMetadata IMethodMetadata.SetAuthentication(AuthenticationType type, string defaultUserName, string relativeUrlToMatch)
        {
            ValidateCurrentServiceMethod();

            m_authenticationDictionary[m_currentServiceMethod] = new AuthenticationMetadata
            {
                Type = type,
                DefaultUserName = defaultUserName,
                RelativeUrlToMatch = relativeUrlToMatch
            };

            return this;
        }

        IMethodMetadata IMethodMetadata.SetDescription(string description)
        {
            ValidateCurrentServiceMethod();

            if (String.IsNullOrEmpty(description))
            {
                throw new ArgumentNullException("description");
            }

            m_descriptionDictionary[m_currentServiceMethod] = description;

            return this;
        }

        IMethodMetadata IMethodMetadata.SetIPFiltered()
        {
            ValidateCurrentServiceMethod();

            m_ipFilteredSet.Add(m_currentServiceMethod);
            return this;
        }

        IMethodMetadata IMethodMetadata.SetHeader(string name, string value)
        {
            ValidateCurrentServiceMethod();

            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            List<HeaderMetadata> headers;

            if (!m_headerDictionary.TryGetValue(m_currentServiceMethod, out headers))
            {
                headers = new List<HeaderMetadata>();
            }

            headers.Add(new HeaderMetadata
            {
                Name = name,
                Value = value
            });

            m_headerDictionary[m_currentServiceMethod] = headers;
            return this;
        }

        IMethodMetadata IMethodMetadata.SetHidden()
        {
            ValidateCurrentServiceMethod();

            m_hiddenOperationSet.Add(m_currentServiceMethod);
            return this;
        }

        IMethodMetadata IMethodMetadata.SetHttps()
        {
            const int DefaultHttpsPort = 443;

            return ((IMethodMetadata) this).SetHttps(DefaultHttpsPort);
        }

        IMethodMetadata IMethodMetadata.SetHttps(int port)
        {
            ValidateCurrentServiceMethod();

            if (port <= 0)
            {
                throw new ArgumentOutOfRangeException("port", "Port must be greater than 0");
            }

            m_httpsDictionary[m_currentServiceMethod] = new HttpsMetadata
            {
                Port = port
            };

            return this;
        }

        IMethodMetadata IMethodMetadata.SetQueryParameter(string name, Type type)
        {
            return ((IMethodMetadata) this).SetQueryParameter(name, type, null, null, null);
        }

        IMethodMetadata IMethodMetadata.SetQueryParameter(string name, Type type, object exampleValue)
        {
            return ((IMethodMetadata) this).SetQueryParameter(name, type, exampleValue, null, null);
        }

        IMethodMetadata IMethodMetadata.SetQueryParameter(string name, Type type, object exampleValue, IList<string> allowedValues)
        {
            return ((IMethodMetadata) this).SetQueryParameter(name, type, exampleValue, allowedValues, null);
        }

        IMethodMetadata IMethodMetadata.SetQueryParameter(string name, Type type, object exampleValue, string regexConstraint)
        {
            return ((IMethodMetadata) this).SetQueryParameter(name, type, exampleValue, null, regexConstraint);
        }

        IMethodMetadata IMethodMetadata.SetQueryParameter(string name, Type type, object exampleValue, IList<string> allowedValues, string regexConstraint)
        {
            ValidateCurrentServiceMethod();

            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            SetParameter(name, type, exampleValue, allowedValues, regexConstraint, false);
            return this;
        }

        IMethodMetadata IMethodMetadata.SetRouteParameter(string name)
        {
            return ((IMethodMetadata) this).SetRouteParameter(name, null, null);
        }

        IMethodMetadata IMethodMetadata.SetRouteParameter(string name, object exampleValue)
        {
            return ((IMethodMetadata) this).SetRouteParameter(name, exampleValue, null);
        }

        IMethodMetadata IMethodMetadata.SetRouteParameter(string name, object exampleValue, IList<string> allowedValues)
        {
            ValidateCurrentServiceMethod();

            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (m_currentServiceMethod.GetParameters().All(p => p.Name != name))
            {
                throw new ArgumentException("Invalid method parameter name provided", "name");
            }

            SetParameter(name, null, exampleValue, allowedValues, null, true);
            return this;
        }

        IMethodMetadata IMethodMetadata.SetRequestResourceExample(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            return ((IMethodMetadata) this).SetRequestResourceExample(instance, XmlSchemaGenerator.Generate(instance.GetType()));
        }

        IMethodMetadata IMethodMetadata.SetRequestResourceExample(object instance, XmlSchemas xmlSchemas)
        {
            ValidateCurrentServiceMethod();

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            m_requestResourceExampleDictionary[m_currentServiceMethod] = new ResourceExampleMetadata
            {
                Instance = instance,
                XmlSchemas = xmlSchemas
            };

            return this;
        }

        IMethodMetadata IMethodMetadata.SetResponseResourceExample(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            return ((IMethodMetadata) this).SetResponseResourceExample(instance, XmlSchemaGenerator.Generate(instance.GetType()));
        }

        IMethodMetadata IMethodMetadata.SetResponseResourceExample(object instance, XmlSchemas xmlSchemas)
        {
            ValidateCurrentServiceMethod();

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            m_responseResourceExampleDictionary[m_currentServiceMethod] = new ResourceExampleMetadata
            {
                Instance = instance,
                XmlSchemas = xmlSchemas
            };

            return this;
        }

        IMethodMetadata IMethodMetadata.SetStatusCode(HttpStatusCode statusCode)
        {
            return ((IMethodMetadata) this).SetStatusCode(statusCode, null);
        }

        IMethodMetadata IMethodMetadata.SetStatusCode(HttpStatusCode statusCode, string statusDescription)
        {
            ValidateCurrentServiceMethod();

            List<StatusCodeMetadata> statusCodes;

            if (!m_statusCodeDictionary.TryGetValue(m_currentServiceMethod, out statusCodes))
            {
                statusCodes = new List<StatusCodeMetadata>();
            }

            statusCodes.Add(new StatusCodeMetadata
            {
                StatusCode = statusCode,
                StatusCondition = statusDescription
            });

            m_statusCodeDictionary[m_currentServiceMethod] = statusCodes;
            return this;
        }

        bool IProxyMetadata.IsIPFiltered(MethodInfo serviceMethod)
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

        bool IProxyMetadata.IsHidden(MethodInfo serviceMethod)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            return m_hiddenOperationSet.Contains(serviceMethod);
        }

        string IProxyMetadata.GetDescription(MethodInfo serviceMethod)
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

        AuthenticationMetadata IProxyMetadata.GetAuthentication(MethodInfo serviceMethod)
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

        HttpsMetadata IProxyMetadata.GetHttps(MethodInfo serviceMethod)
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

        ResourceExampleMetadata IProxyMetadata.GetRequestResourceExample(MethodInfo serviceMethod)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            ResourceExampleMetadata resourceExample;

            if (!m_requestResourceExampleDictionary.TryGetValue(serviceMethod, out resourceExample))
            {
                return null;
            }

            return resourceExample;
        }

        ResourceExampleMetadata IProxyMetadata.GetResponseResourceExample(MethodInfo serviceMethod)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            ResourceExampleMetadata resourceExample;

            if (!m_responseResourceExampleDictionary.TryGetValue(serviceMethod, out resourceExample))
            {
                return null;
            }

            return resourceExample;
        }

        public ParameterMetadata GetParameter(MethodInfo serviceMethod, string name, bool isRouteParameter)
        {
            List<ParameterMetadata> parameters;

            if (!m_parameterDictionary.TryGetValue(serviceMethod, out parameters))
            {
                return null;
            }

            return parameters.FirstOrDefault(p => p.Name == name && p.IsRouteParameter == isRouteParameter);
        }

        IList<ParameterMetadata> IProxyMetadata.GetParameters(MethodInfo serviceMethod, bool isRouteParameter)
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

            return parameters.Where(p => p.IsRouteParameter == isRouteParameter).ToList();
        }

        IList<HeaderMetadata> IProxyMetadata.GetHeaders(MethodInfo serviceMethod)
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

        IList<StatusCodeMetadata> IProxyMetadata.GetStatusCodes(MethodInfo serviceMethod)
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

        private void ValidateCurrentServiceMethod()
        {
            if (m_currentServiceMethod == null)
            {
                throw new InvalidOperationException("No current service method has been set");
            }
        }

        private void SetParameter(string name, Type type, object exampleValue, IEnumerable<string> allowedValues, string regexConstraint, bool isRouteParameter)
        {
            List<ParameterMetadata> parameters;

            if (!m_parameterDictionary.TryGetValue(m_currentServiceMethod, out parameters))
            {
                parameters = new List<ParameterMetadata>();
            }

            parameters.Add(new ParameterMetadata
            {
                Name = name,
                Type = type,
                ExampleValue = exampleValue,
                AllowedValues = allowedValues != null ? String.Join(", ", allowedValues.Where(o => o != null).ToArray()) : null,
                RegexConstraint = regexConstraint,
                IsRouteParameter = isRouteParameter
            });

            m_parameterDictionary[m_currentServiceMethod] = parameters;
        }
    }
}
