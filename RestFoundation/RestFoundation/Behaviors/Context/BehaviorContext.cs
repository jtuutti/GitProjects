// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Reflection;
using RestFoundation.Runtime;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// The base class for a behavior context.
    /// This class cannot be instantiated.
    /// </summary>
    public abstract class BehaviorContext
    {
        private readonly object m_service;
        private readonly MethodInfo m_method;

        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviorContext"/> class.
        /// </summary>
        protected BehaviorContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviorContext"/> class.
        /// </summary>
        /// <param name="service">The service instance.</param>
        /// <param name="method">The service method.</param>
        protected BehaviorContext(object service, MethodInfo method)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            m_service = service;
            m_method = method;
        }

        /// <summary>
        /// Gets the service instance.
        /// </summary>
        public virtual object Service
        {
            get
            {
                return m_service;
            }
        }

        /// <summary>
        /// Gets the service method.
        /// </summary>
        public virtual MethodInfo Method
        {
            get
            {
                return m_method;
            }
        }

        /// <summary>
        /// Gets the service contract type.
        /// </summary>
        /// <returns>The service contract type.</returns>
        public virtual Type GetServiceContractType()
        {
            if (m_service == null)
            {
                return null;
            }

            Type serviceType = m_service.GetType();

            if (Attribute.IsDefined(serviceType, typeof(ServiceContractAttribute), true))
            {
                return serviceType;
            }

            ICollection<Type> registeredServiceInterfaces = ServiceContractTypeRegistry.GetContractTypes();
            Type[] serviceInterfaces = serviceType.GetInterfaces();

            foreach (Type serviceInterface in serviceInterfaces)
            {
                if (registeredServiceInterfaces.Contains(serviceInterface))
                {
                    return serviceInterface;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the service type.
        /// </summary>
        /// <returns>The service type.</returns>
        public virtual Type GetServiceType()
        {
            return m_service != null ? m_service.GetType() : null;
        }

        /// <summary>
        /// Gets the service method name.
        /// </summary>
        /// <returns>The service method name.</returns>
        public virtual string GetMethodName()
        {
            return m_method != null ? m_method.Name : null;
        }

        /// <summary>
        /// Gets supported HTTP methods by the service method.
        /// </summary>
        /// <returns>A sequence of supported HTTP methods.</returns>
        public virtual IEnumerable<HttpMethod> GetSupportedHttpMethods()
        {
            if (m_method == null)
            {
                return new HttpMethod[0];
            }

            var httpMethodResolver = Rest.Configuration.ServiceLocator.GetService<IHttpMethodResolver>();

            return httpMethodResolver.Resolve(m_method);
        }

        /// <summary>
        /// Gets the URL template for the service method.
        /// </summary>
        /// <returns>The URL template.</returns>
        public virtual string GetUrlTemplate()
        {
            if (m_method == null)
            {
                return null;
            }

            var urlAttribute = Attribute.GetCustomAttribute(m_method, typeof(UrlAttribute), false) as UrlAttribute;

            return urlAttribute != null ? urlAttribute.UrlTemplate : null;
        }
    }
}
