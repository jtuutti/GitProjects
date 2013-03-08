// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Practices.Unity;
using RestFoundation.ServiceLocation;

namespace RestFoundation.Unity
{
    /// <summary>
    /// Represents a service locator that abstracts a Unity container.
    /// </summary>
    public sealed class ServiceLocator : IServiceLocator
    {
        private readonly IUnityContainer m_container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocator"/> class with the provided Unity container.
        /// </summary>
        /// <param name="container">The container.</param>
        public ServiceLocator(IUnityContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");

            m_container = container;
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <filterpriority>2</filterpriority>
        public object GetService(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException("serviceType");

            try
            {
                if (!serviceType.IsInterface && !serviceType.IsAbstract)
                {
                    return m_container.Resolve(serviceType);
                }

                if (m_container.IsRegistered(serviceType))
                {
                    return m_container.Resolve(serviceType);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Properties.Resources.DependencyResolutionError, ex.Message), ex);
            }
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">A type of service object to get.</typeparam>
        /// <returns>
        /// A service object of type <typeparamref name="T"/>.-or- null if there is no service object
        /// of type <typeparamref name="T"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public T GetService<T>()
        {
            Type serviceType = typeof(T);

            try
            {
                if (!serviceType.IsInterface && !serviceType.IsAbstract)
                {
                    return m_container.Resolve<T>();
                }

                if (m_container.IsRegistered<T>())
                {
                    return m_container.Resolve<T>();
                }

                return default(T);
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Properties.Resources.DependencyResolutionError, ex.Message), ex);
            }
        }

        /// <summary>
        /// Gets all the service objects of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service objects to get.</param>
        /// <returns>
        /// A sequence of service objects of type <paramref name="serviceType"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException("serviceType");

            try
            {
                var services = new List<object>(m_container.ResolveAll(serviceType));
                
                object defaultService = GetService(serviceType); // by default Unity does not return unnamed instances

                if (defaultService != null && !services.Contains(defaultService))
                {
                    services.Insert(0, defaultService);
                }

                return services;
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Properties.Resources.DependencyResolutionError, ex.Message), ex);
            }
        }

        /// <summary>
        /// Gets all the service objects of the specified type.
        /// </summary>
        /// <typeparam name="T">A type of service objects to get.</typeparam>
        /// <returns>
        /// A sequence of service objects of type <typeparamref name="T"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public IEnumerable<T> GetServices<T>()
        {
            try
            {
                var services = new List<T>(m_container.ResolveAll<T>());

                var defaultService = GetService<T>(); // by default Unity does not return unnamed instances

                if (!Equals(defaultService, default(T)) && !services.Contains(defaultService))
                {
                    services.Insert(0, defaultService);
                }

                return services;
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Properties.Resources.DependencyResolutionError, ex.Message), ex);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            m_container.Dispose();
        }
    }
}
