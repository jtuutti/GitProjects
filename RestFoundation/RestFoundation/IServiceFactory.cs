using System;

namespace RestFoundation
{
    /// <summary>
    /// Defines a service factory.
    /// </summary>
    public interface IServiceFactory
    {
        /// <summary>
        /// Creates a service object instance from the service contract type.
        /// </summary>
        /// <param name="serviceContractType">The service contract type.</param>
        /// <param name="context">The service context.</param>
        /// <returns>The service instance.</returns>
        object Create(Type serviceContractType, IServiceContext context);
    }
}
