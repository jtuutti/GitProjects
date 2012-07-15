using System;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents a default service factory.
    /// </summary>
    public class ServiceFactory : IServiceFactory
    {
        /// <summary>
        /// Creates a service object instance from the service contract type.
        /// </summary>
        /// <param name="serviceContractType">The service contract type.</param>
        /// <param name="context">The service context.</param>
        /// <returns>The service instance.</returns>
        public virtual object Create(Type serviceContractType, IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            return Rest.Active.CreateObject(serviceContractType);
        }
    }
}
