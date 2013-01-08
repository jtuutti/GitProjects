// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;

namespace RestFoundation.Client
{
    /// <summary>
    /// Defines a REST serializer factory.
    /// </summary>
    public interface IRestSerializerFactory
    {
        /// <summary>
        /// Returns a serializer that matches the object type and the resource content type.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <param name="resourceType">The resource type.</param>
        /// <returns>The serializer instance.</returns>
        IRestSerializer Create(Type objectType, RestResourceType resourceType);
    }
}
