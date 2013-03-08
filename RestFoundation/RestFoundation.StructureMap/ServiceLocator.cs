// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RestFoundation.ServiceLocation;
using StructureMap;

namespace RestFoundation.StructureMap
{
    /// <summary>
    /// Represents a service locator that abstracts a StructureMap container.
    /// </summary>
    public sealed class ServiceLocator : IServiceLocator
    {
        private readonly IContainer m_container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocator"/> class with the provided StructureMap container.
        /// </summary>
        /// <param name="container">The container.</param>
        public ServiceLocator(IContainer container)
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
        /// <param name="serviceType">An object that specifies the type of service object to get. </param>
        /// <filterpriority>2</filterpriority>
        public object GetService(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException("serviceType");

            try
            {
                return (serviceType.IsInterface || serviceType.IsAbstract) ? m_container.TryGetInstance(serviceType) : m_container.GetInstance(serviceType);
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
                return (serviceType.IsInterface || serviceType.IsAbstract) ? m_container.TryGetInstance<T>() : m_container.GetInstance<T>();
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
                return m_container.GetAllInstances(serviceType).Cast<object>();
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
                return m_container.GetAllInstances<T>();
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Properties.Resources.DependencyResolutionError, ex.Message), ex);
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
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, Properties.Resources.DependencyBuildUpError, ex.Message), ex);
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
