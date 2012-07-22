using System;
using System.Collections.Generic;

namespace RestFoundation.ServiceLocation
{
    /// <summary>
    /// Defines a service locator to retrieve services from an IoC container.
    /// </summary>
    public interface IServiceLocator : IServiceProvider, IDisposable
    {
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">A type of service object to get.</typeparam>
        /// <returns>
        /// A service object of type <typeparamref name="T"/>.-or- null if there is no service object
        /// of type <typeparamref name="T"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        T GetService<T>();

        /// <summary>
        /// Gets all the service objects of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service objects to get.</param>
        /// <returns>
        /// A sequence of service objects of type <paramref name="serviceType"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerable<object> GetServices(Type serviceType);

        /// <summary>
        /// Gets all the service objects of the specified type.
        /// </summary>
        /// <typeparam name="T">A type of service objects to get.</typeparam>
        /// <returns>
        /// A sequence of service objects of type <typeparamref name="T"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerable<T> GetServices<T>();

        /// <summary>
        /// Takes an already constructed object and injects service dependencies into its properties.
        /// </summary>
        /// <param name="obj">The object.</param>
        void BuildUp(object obj);
    }
}
