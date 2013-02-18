// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;

namespace RestFoundation.Client.Serializers
{
    /// <summary>
    /// Represents a REST serializer factory that supports JSON and XML.
    /// </summary>
    public class RestSerializerFactory : IRestSerializerFactory
    {
        /// <summary>
        /// Creates a serializer instance based on the object type and the REST resource type.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <param name="resourceType">The resource type.</param>
        /// <param name="xmlNamespace">An optional XML resource namespace.</param>
        /// <returns>The serializer instance.</returns>
        public virtual IRestSerializer Create(Type objectType, RestResourceType resourceType, string xmlNamespace)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType");
            }

            if (objectType == typeof(string))
            {
                return new StringSerializer();
            }

            switch (resourceType)
            {
                case RestResourceType.Json:
                    return new JsonObjectSerializer();
                case RestResourceType.Xml:
                    return new XmlObjectSerializer(xmlNamespace);
            }

            throw new NotSupportedException();
        }
    }
}
