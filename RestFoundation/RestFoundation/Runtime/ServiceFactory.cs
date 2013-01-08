// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents the default service factory.
    /// </summary>
    public class ServiceFactory : IServiceFactory
    {
        /// <summary>
        /// Creates a service implementation instance.
        /// </summary>
        /// <param name="serviceContractType">A service contract type.</param>
        /// <returns>The created service instance.</returns>
        public object Create(Type serviceContractType)
        {
            if (serviceContractType == null)
            {
                throw new ArgumentNullException("serviceContractType");
            }

            return Rest.Configuration.ServiceLocator.GetService(serviceContractType);
        }
    }
}
