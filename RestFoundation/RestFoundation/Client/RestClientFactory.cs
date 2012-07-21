using System;
using System.Collections.Generic;

namespace RestFoundation.Client
{
    /// <summary>
    /// Creates <see cref="IRestClient"/> instances.
    /// </summary>
    public static class RestClientFactory
    {
        private static readonly TimeSpan defaultConnectionTimeout = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan defaultSocketTimeout = TimeSpan.FromMinutes(10);
        private static readonly IDictionary<RestResourceType, string> defaultResourceTypes = new Dictionary<RestResourceType, string>
                                                                                             {
                                                                                                 { RestResourceType.Json, "application/json" },
                                                                                                 { RestResourceType.Xml, "text/xml" }
                                                                                             };

        /// <summary>
        /// Creates a new <see cref="IRestClient"/> instance with the default resource types and timeouts.
        /// </summary>
        /// <returns>The created <see cref="IRestClient"/> instance.</returns>
        public static IRestClient Create()
        {
            return Create(defaultConnectionTimeout, defaultSocketTimeout);
        }

        /// <summary>
        /// Creates a new <see cref="IRestClient"/> instance with the default resource types.
        /// </summary>
        /// <param name="connectionTimeout">A connection timeout.</param>
        /// <param name="socketTimeout">A socket timeout.</param>
        /// <returns>The created <see cref="IRestClient"/> instance.</returns>
        public static IRestClient Create(TimeSpan connectionTimeout, TimeSpan socketTimeout)
        {
            return Create(connectionTimeout, socketTimeout, defaultResourceTypes);
        }

        /// <summary>
        /// Creates a new <see cref="IRestClient"/> instance with the default timeouts.
        /// </summary>
        /// <param name="resourceTypes">A dictionary of resource types mapped to MIME content types.</param>
        /// <returns>The created <see cref="IRestClient"/> instance.</returns>
        public static IRestClient Create(IDictionary<RestResourceType, string> resourceTypes)
        {
            return Create(defaultConnectionTimeout, defaultSocketTimeout, resourceTypes);
        }

        /// <summary>
        /// Creates a new <see cref="IRestClient"/> instance.
        /// </summary>
        /// <param name="connectionTimeout">A connection timeout.</param>
        /// <param name="socketTimeout">A socket timeout.</param>
        /// <param name="resourceTypes">A dictionary of resource types mapped to MIME content types.</param>
        /// <returns>The created <see cref="IRestClient"/> instance.</returns>
        public static IRestClient Create(TimeSpan connectionTimeout, TimeSpan socketTimeout, IDictionary<RestResourceType, string> resourceTypes)
        {
            IRestClient client = new RestClient(Rest.Active.ServiceLocator.GetService<IRestSerializerFactory>(), resourceTypes)
            {
                ConnectionTimeout = connectionTimeout,
                SocketTimeout = socketTimeout
            };

            return client;
        }
    }
}
