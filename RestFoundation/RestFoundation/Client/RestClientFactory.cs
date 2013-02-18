// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System.Collections.Generic;

namespace RestFoundation.Client
{
    /// <summary>
    /// Creates <see cref="IRestClient"/> instances.
    /// </summary>
    public static class RestClientFactory
    {
        private static readonly IDictionary<RestResourceType, string> defaultResourceTypes = new Dictionary<RestResourceType, string>
                                                                                             {
                                                                                                 { RestResourceType.Json, "application/json" },
                                                                                                 { RestResourceType.Xml, "application/xml" }
                                                                                             };

        /// <summary>
        /// Creates a new <see cref="IRestClient"/> instance with the default resource types and timeouts.
        /// </summary>
        /// <returns>The created <see cref="IRestClient"/> instance.</returns>
        public static IRestClient Create()
        {
            return Create(null);
        }

        /// <summary>
        /// Creates a new <see cref="IRestClient"/> instance.
        /// </summary>
        /// <param name="resourceTypes">A dictionary of resource types mapped to MIME content types.</param>
        /// <returns>The created <see cref="IRestClient"/> instance.</returns>
        public static IRestClient Create(IDictionary<RestResourceType, string> resourceTypes)
        {
            var serializerFactory = Rest.Configuration.ServiceLocator.GetService<IRestSerializerFactory>();

            return new RestClient(serializerFactory, resourceTypes ?? defaultResourceTypes);
        }
    }
}
