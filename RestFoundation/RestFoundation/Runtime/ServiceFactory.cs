// <copyright>
// Dmitry Starosta, 2012-2014
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
        /// <param name="serviceContractType">The service contract type.</param>
        /// <param name="request">The HTTP request.</param>
        /// <returns>The created service instance.</returns>
        public virtual object Create(Type serviceContractType, IHttpRequest request)
        {
            if (serviceContractType == null)
            {
                throw new ArgumentNullException("serviceContractType");
            }

            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            return Rest.Configuration.ServiceLocator.GetService(serviceContractType);
        }
    }
}
