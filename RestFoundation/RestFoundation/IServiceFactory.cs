// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;

namespace RestFoundation
{
    /// <summary>
    /// Defines a service factory.
    /// </summary>
    public interface IServiceFactory
    {
        /// <summary>
        /// Creates a service implementation instance.
        /// </summary>
        /// <param name="serviceContractType">A service contract type.</param>
        /// <returns>The created service instance.</returns>
        object Create(Type serviceContractType);
    }
}
