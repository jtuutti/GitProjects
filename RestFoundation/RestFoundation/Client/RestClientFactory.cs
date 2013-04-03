// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using RestFoundation.Client.Serializers;

namespace RestFoundation.Client
{
    using ClientBuilder = Func<IRestSerializerFactory, IDictionary<RestResourceType, string>, IRestClient>;

    /// <summary>
    /// Creates <see cref="IRestClient"/> instances.
    /// </summary>
    public static class RestClientFactory
    {
        private static ClientBuilder builder = (serializerFactory, resourceTypes) => new RestClient(serializerFactory ?? new RestSerializerFactory(),
                                                                                     resourceTypes ?? new Dictionary<RestResourceType, string>
                                                                                                      {
                                                                                                          { RestResourceType.Json, "application/json" },
                                                                                                          { RestResourceType.Xml, "application/xml" }
                                                                                                      });

        /// <summary>
        /// Gets or sets a delegate to create a REST client.
        /// </summary>
        public static ClientBuilder Builder
        {
            get
            {
                return builder;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                builder = value;
            }
        }

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
            return Builder(null, resourceTypes);
        }
    }
}
