// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RestFoundation.ServiceLocation;
using TinyIoC;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents a service locator that abstracts the default Rest Foundation container.
    /// </summary>
    public sealed class DefaultServiceLocator : IServiceLocator
    {
        private readonly TinyIoCContainer m_container;
        private readonly Func<Type, bool> m_propertyInjectionPredicate;

        internal DefaultServiceLocator(TinyIoCContainer container, Func<Type, bool> propertyInjectionPredicate)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            m_container = container;
            m_propertyInjectionPredicate = propertyInjectionPredicate ?? (t => false);
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
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

            try
            {
                object resolvedObject;

                if (!m_container.TryResolve(serviceType, out resolvedObject))
                {
                    return null;
                }

                BuildUpServiceIfNecessary(new[] { resolvedObject });

                return resolvedObject;
            }
            catch (Exception ex)
            {
                throw new ServiceActivationException(String.Format(CultureInfo.InvariantCulture, RestResources.DependencyResolutionError, ex.Message), ex);
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
            return (T) GetService(typeof(T));
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
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

            var services = m_container.ResolveAll(serviceType).ToArray();
            BuildUpServiceIfNecessary(services);

            return services;
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
            var services = m_container.ResolveAll(typeof(T)).ToArray();
            BuildUpServiceIfNecessary(services);

            return services.Cast<T>();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            m_container.Dispose();
        }

        private void BuildUpServiceIfNecessary(IEnumerable<object> resolvedObjects)
        {
            foreach (object resolvedObject in resolvedObjects)
            {
                if (resolvedObject != null && m_propertyInjectionPredicate(resolvedObject.GetType()))
                {
                    m_container.BuildUp(resolvedObject);
                }
            }
        }
    }
}
