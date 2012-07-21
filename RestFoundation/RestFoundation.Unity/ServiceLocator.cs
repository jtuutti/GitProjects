using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Practices.Unity;
using RestFoundation.ServiceLocation;
using RestFoundation.Unity.Properties;

namespace RestFoundation.Unity
{
    /// <summary>
    /// Represents a service locator that abstracts a Unity container.
    /// </summary>
    public sealed class ServiceLocator : IServiceLocator, IDisposable
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
        /// <param name="serviceType">An object that specifies the type of service object to get. </param><filterpriority>2</filterpriority>
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
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyResolutionError, ex.Message), ex);
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
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyResolutionError, ex.Message), ex);
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
                return m_container.ResolveAll(serviceType);
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyResolutionError, ex.Message), ex);
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
                return m_container.ResolveAll<T>();
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyResolutionError, ex.Message), ex);
            }
        }

        /// <summary>
        /// Takes an already constructed object and injects service dependencies into its properties.
        /// </summary>
        /// <param name="obj">The object.</param>
        public void BuildUp(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            try
            {
                m_container.BuildUp(obj);
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyBuildUpError, ex.Message), ex);
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
