using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Xml.Serialization;
using RestFoundation.Runtime;
using RestFoundation.ServiceProxy.OperationMetadata;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a service proxy metadata for a service. This class cannot be instantiated.
    /// </summary>
    /// <typeparam name="TContract">The service contract type.</typeparam>
    public abstract class ProxyMetadata<TContract> : IMethodMetadata, IProxyMetadata
        where TContract : class
    {
        private const int DefaultHttpsPort = 443;

        private readonly HashSet<MethodInfo> m_hiddenOperationSet = new HashSet<MethodInfo>();
        private readonly HashSet<MethodInfo> m_ipFilteredSet = new HashSet<MethodInfo>();
        private readonly SortedSet<HeaderMetadata> m_serviceHeaders = new SortedSet<HeaderMetadata>();

        private readonly Dictionary<MethodInfo, string> m_descriptionDictionary = new Dictionary<MethodInfo, string>();
        private readonly Dictionary<MethodInfo, AuthenticationMetadata> m_authenticationDictionary = new Dictionary<MethodInfo, AuthenticationMetadata>();
        private readonly Dictionary<MethodInfo, HttpsMetadata> m_httpsDictionary = new Dictionary<MethodInfo, HttpsMetadata>();
        private readonly Dictionary<MethodInfo, ResourceExampleMetadata> m_requestResourceExampleDictionary = new Dictionary<MethodInfo, ResourceExampleMetadata>();
        private readonly Dictionary<MethodInfo, ResourceExampleMetadata> m_responseResourceExampleDictionary = new Dictionary<MethodInfo, ResourceExampleMetadata>();

        private readonly Dictionary<MethodInfo, SortedSet<HeaderMetadata>> m_headerDictionary = new Dictionary<MethodInfo, SortedSet<HeaderMetadata>>();
        private readonly Dictionary<MethodInfo, HashSet<ParameterMetadata>> m_parameterDictionary = new Dictionary<MethodInfo, HashSet<ParameterMetadata>>();
        private readonly Dictionary<MethodInfo, SortedSet<StatusCodeMetadata>> m_statusCodeDictionary = new Dictionary<MethodInfo, SortedSet<StatusCodeMetadata>>();

        private AuthenticationMetadata m_authentication;
        private HttpsMetadata m_https;
        private bool m_isIPFiltered, m_isInitialized;

        private MethodInfo m_currentServiceMethod;

        /// <summary>
        /// Sets the global service authentication.
        /// </summary>
        /// <param name="type">The authentication type.</param>
        public void SetAuthentication(AuthenticationType type)
        {
            SetAuthentication(type, null, null);
        }

        /// <summary>
        /// Sets the global service authentication.
        /// </summary>
        /// <param name="type">The authentication type.</param>
        /// <param name="defaultUserName">The default user name.</param>
        public void SetAuthentication(AuthenticationType type, string defaultUserName)
        {
            SetAuthentication(type, defaultUserName, null);
        }

        /// <summary>
        /// Sets the global service authentication.
        /// </summary>
        /// <param name="type">The authentication type.</param>
        /// <param name="defaultUserName">The default user name.</param>
        /// <param name="relativeUrlToMatch">A relative URL to apply authentication to.</param>
        public void SetAuthentication(AuthenticationType type, string defaultUserName, string relativeUrlToMatch)
        {
            m_authentication = new AuthenticationMetadata
            {
                Type = type,
                DefaultUserName = defaultUserName,
                RelativeUrlToMatch = relativeUrlToMatch
            };
        }

        /// <summary>
        /// Sets an additional HTTP header for all service operations.
        /// </summary>
        /// <param name="header">The HTTP header for the proxy.</param>
        public void SetHeader(ProxyHeader header)
        {
            SetHeader(header.Name, header.Value);
        }

        /// <summary>
        /// Sets an additional HTTP header for all service operations.
        /// </summary>
        /// <param name="name">The HTTP header name.</param>
        /// <param name="value">The HTTP header value.</param>
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

        /// <summary>
        /// Sets additional HTTP headers for all service operations.
        /// </summary>
        /// <param name="headers">The list of HTTP headers for the proxy.</param>
        public void SetHeaders(IList<ProxyHeader> headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            foreach (ProxyHeader header in headers)
            {
                if (String.IsNullOrWhiteSpace(header.Name) || String.IsNullOrEmpty(header.Value))
                {
                    continue;
                }

                m_serviceHeaders.Add(new HeaderMetadata
                {
                    Name = header.Name,
                    Value = header.Value
                });
            }
        }

        /// <summary>
        /// Sets all service operations to require secure HTTPS connection with port 443.
        /// </summary>
        public void SetHttps()
        {
            SetHttps(DefaultHttpsPort);
        }

        /// <summary>
        /// Sets all service operations as HTTPS specific.
        /// </summary>
        /// <param name="port">The TCP port number.</param>
        public void SetHttps(int port)
        {
            if (port <= 0)
            {
                throw new ArgumentOutOfRangeException("port", RestResources.InvalidPortNumber);
            }

            m_https = new HttpsMetadata
            {
                Port = port
            };
        }
        
        /// <summary>
        /// Marks all service operations as IP filtered.
        /// </summary>
        public void SetIPFiltered()
        {
            m_isIPFiltered = true;
        }

        /// <summary>
        /// Specifies metadata for a specific service method.
        /// </summary>
        /// <param name="serviceMethod">The service method.</param>
        /// <returns>The service method metadata.</returns>
        public IMethodMetadata ForMethod(Expression<Func<TContract, object>> serviceMethod)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            var methodExpression = serviceMethod.Body as MethodCallExpression;

            if (methodExpression == null || methodExpression.Method == null)
            {
                throw new ArgumentException(RestResources.InvalidServiceMethodExpression, "serviceMethod");
            }

            m_currentServiceMethod = methodExpression.Method;

            var arguments = ExpressionArgumentExtractor.Extract(methodExpression);

            foreach (var argument in arguments)
            {
                ((IMethodMetadata) this).SetRouteParameter(argument.Name, argument.Value);
            }

            return this;
        }

        /// <summary>
        /// Initializes the service contract metadata.
        /// </summary>
        public abstract void Initialize();

        #region IMethodMetadata Members

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

        IMethodMetadata IMethodMetadata.SetHeader(ProxyHeader header)
        {
            return ((IMethodMetadata) this).SetHeader(header.Name, header.Value);
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

            SortedSet<HeaderMetadata> methodHeaders;

            if (!m_headerDictionary.TryGetValue(m_currentServiceMethod, out methodHeaders))
            {
                methodHeaders = new SortedSet<HeaderMetadata>();
            }

            methodHeaders.Add(new HeaderMetadata
            {
                Name = name,
                Value = value
            });

            m_headerDictionary[m_currentServiceMethod] = methodHeaders;
            return this;
        }

        IMethodMetadata IMethodMetadata.SetHeaders(IList<ProxyHeader> headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            SortedSet<HeaderMetadata> methodHeaders;

            if (!m_headerDictionary.TryGetValue(m_currentServiceMethod, out methodHeaders))
            {
                methodHeaders = new SortedSet<HeaderMetadata>();
            }

            foreach (ProxyHeader header in headers)
            {
                if (String.IsNullOrWhiteSpace(header.Name) || String.IsNullOrEmpty(header.Value))
                {
                    continue;
                }

                methodHeaders.Add(new HeaderMetadata
                {
                    Name = header.Name,
                    Value = header.Value
                });
            }

            m_headerDictionary[m_currentServiceMethod] = methodHeaders;
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
            return ((IMethodMetadata) this).SetHttps(DefaultHttpsPort);
        }

        IMethodMetadata IMethodMetadata.SetHttps(int port)
        {
            ValidateCurrentServiceMethod();

            if (port <= 0)
            {
                throw new ArgumentOutOfRangeException("port", RestResources.InvalidPortNumber);
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
                throw new ArgumentException(RestResources.InvalidMethodParameterName, "name");
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

        IMethodMetadata IMethodMetadata.SetResponseStatus(ProxyStatus status)
        {
            return ((IMethodMetadata) this).SetResponseStatus(status.Code, status.Condition);
        }

        IMethodMetadata IMethodMetadata.SetResponseStatus(HttpStatusCode statusCode)
        {
            return ((IMethodMetadata) this).SetResponseStatus(statusCode, statusCode.ToString());
        }

        IMethodMetadata IMethodMetadata.SetResponseStatus(HttpStatusCode statusCode, string statusDescription)
        {
            if (!Enum.IsDefined(typeof(HttpStatusCode), statusCode))
            {
                throw new ArgumentOutOfRangeException("statusCode");
            }

            if (String.IsNullOrEmpty(statusDescription))
            {
                throw new ArgumentNullException("statusDescription");
            }

            ValidateCurrentServiceMethod();

            SortedSet<StatusCodeMetadata> methodStatuses;

            if (!m_statusCodeDictionary.TryGetValue(m_currentServiceMethod, out methodStatuses))
            {
                methodStatuses = new SortedSet<StatusCodeMetadata>();
            }

            methodStatuses.Add(new StatusCodeMetadata
            {
                Code = statusCode,
                Condition = statusDescription
            });

            m_statusCodeDictionary[m_currentServiceMethod] = methodStatuses;
            return this;
        }

        IMethodMetadata IMethodMetadata.SetResponseStatuses(IList<ProxyStatus> statuses)
        {
            if (statuses == null)
            {
                throw new ArgumentNullException("statuses");
            }

            SortedSet<StatusCodeMetadata> methodStatuses;

            if (!m_statusCodeDictionary.TryGetValue(m_currentServiceMethod, out methodStatuses))
            {
                methodStatuses = new SortedSet<StatusCodeMetadata>();
            }

            foreach (ProxyStatus status in statuses)
            {
                if (String.IsNullOrEmpty(status.Condition))
                {
                    continue;
                }

                methodStatuses.Add(new StatusCodeMetadata
                {
                    Code = status.Code,
                    Condition = status.Condition
                });
            }

            m_statusCodeDictionary[m_currentServiceMethod] = methodStatuses;
            return this;
        }

        #endregion

        #region IProxyMetadata Members

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

        ParameterMetadata IProxyMetadata.GetParameter(MethodInfo serviceMethod, string name, bool isRouteParameter)
        {
            HashSet<ParameterMetadata> parameters;

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

            HashSet<ParameterMetadata> parameters;

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

            SortedSet<HeaderMetadata> headers, operationHeaders;

            if (!m_headerDictionary.TryGetValue(serviceMethod, out operationHeaders))
            {
                headers = new SortedSet<HeaderMetadata>(m_serviceHeaders);
            }
            else
            {
                headers = new SortedSet<HeaderMetadata>(operationHeaders.Union(m_serviceHeaders));
            }

            return headers.ToList();          
        }

        IList<StatusCodeMetadata> IProxyMetadata.GetResponseStatuses(MethodInfo serviceMethod)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            SortedSet<StatusCodeMetadata> statusCodes;

            if (!m_statusCodeDictionary.TryGetValue(serviceMethod, out statusCodes))
            {
                return new List<StatusCodeMetadata>();
            }

            return statusCodes.ToList();
        }

        void IProxyMetadata.Initialize()
        {
            if (m_isInitialized)
            {
                return;
            }

            Initialize();
            m_isInitialized = true;
        }

        #endregion

        #region Private Methods

        private void ValidateCurrentServiceMethod()
        {
            if (m_currentServiceMethod == null)
            {
                throw new InvalidOperationException(RestResources.MissingCurrentServiceMethod);
            }
        }

        private void SetParameter(string name, Type type, object exampleValue, IEnumerable<string> allowedValues, string regexConstraint, bool isRouteParameter)
        {
            HashSet<ParameterMetadata> parameters;

            if (!m_parameterDictionary.TryGetValue(m_currentServiceMethod, out parameters))
            {
                parameters = new HashSet<ParameterMetadata>();
            }

            var parameter = new ParameterMetadata
            {
                Name = name,
                Type = type,
                ExampleValue = exampleValue is bool ? exampleValue.ToString().ToLowerInvariant() : exampleValue,
                AllowedValues = allowedValues != null ? String.Join(", ", allowedValues.Where(o => o != null).ToArray()) : null,
                RegexConstraint = regexConstraint,
                IsRouteParameter = isRouteParameter
            };

            // trying to delete existing parameters to allow overridding inferred values from the lambda expressions
            parameters.Remove(parameter);

            parameters.Add(parameter);

            m_parameterDictionary[m_currentServiceMethod] = parameters;
        }

        #endregion
    }
}
