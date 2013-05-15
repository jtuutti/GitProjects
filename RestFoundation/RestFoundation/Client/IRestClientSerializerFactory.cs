// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;

namespace RestFoundation.Client
{
    /// <summary>
    /// Defines a REST client serializer factory.
    /// </summary>
    public interface IRestClientSerializerFactory
    {
        /// <summary>
        /// Returns a serializer that matches the object type and the resource content type.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <param name="resourceType">The resource type.</param>
        /// <param name="xmlNamespace">An optional XML resource namespace.</param>
        /// <returns>The serializer instance.</returns>
        IRestClientSerializer Create(Type objectType, RestResourceType resourceType, string xmlNamespace);
    }
}
