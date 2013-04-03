﻿// <copyright>
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
        private static readonly ClientBuilder defaultBuilder = (serializerFactory, resourceTypes) => new RestClient(serializerFactory ?? new RestSerializerFactory(),
                                                                                                                    resourceTypes ?? new Dictionary<RestResourceType, string>
                                                                                                                    {
                                                                                                                        { RestResourceType.Json, "application/json" },
                                                                                                                        { RestResourceType.Xml, "application/xml" }
                                                                                                                    });
        private static ClientBuilder currentBuilder;

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
<<<<<<< HEAD
            return (currentBuilder ?? defaultBuilder)(null, resourceTypes);
        }

        /// <summary>
        /// Sets a delegate to create a REST client instance.
        /// </summary>
        /// <param name="builder">A REST client builder delegate.</param>
        /// <returns>
        /// An <see cref="T:System.IDisposable"/> to reset the default REST client builder.
        /// </returns>
        public static IDisposable UseBuilder(ClientBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            currentBuilder = builder;

            return new BuilderResetter();
        }

        private sealed class BuilderResetter : IDisposable
        {
            public void Dispose()
            {
                currentBuilder = null;
            }
=======
            return Builder(null, resourceTypes);
>>>>>>> 310cee91cd95e222d5b22a727641646ae8ec6443
        }
    }
}
