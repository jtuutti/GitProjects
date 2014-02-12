// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RestFoundation.Runtime
{
    internal static class XmlClientSerializerRegistry
    {
        private static readonly ConcurrentDictionary<Type, XmlSerializer> serializers = new ConcurrentDictionary<Type, XmlSerializer>();
        private static readonly object syncRoot = new Object();

        public static XmlSerializer Get(Type objectType, string rootNamespace)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType");
            }

            return serializers.GetOrAdd(objectType, InitializeXmlSerializer(objectType, rootNamespace));
        }

        private static XmlSerializer InitializeXmlSerializer(Type objectType, string rootNamespace)
        {
            lock (syncRoot)
            {
                if (objectType.IsArray)
                {
                    return GetSerializerForCollection(objectType, objectType.GetElementType(), rootNamespace);
                }

                if (objectType.IsGenericType && objectType.GetGenericTypeDefinition().GetInterface(typeof(IEnumerable<>).FullName) != null)
                {
                    return GetSerializerForCollection(objectType, objectType.GetGenericArguments()[0], rootNamespace);
                }

                return !String.IsNullOrWhiteSpace(rootNamespace) ? new XmlSerializer(objectType, rootNamespace) : new XmlSerializer(objectType);
            }
        }

        private static XmlSerializer GetSerializerForCollection(Type objectType, Type elementType, string rootNamespace)
        {
            string elementRootNamespace;
            string rootElementName = XmlRootElementInspector.GetRootElementName(elementType, out elementRootNamespace);

            if (String.IsNullOrWhiteSpace(elementRootNamespace))
            {
                elementRootNamespace = rootNamespace;
            }

            XmlSerializer serializer;

            if (!String.IsNullOrWhiteSpace(elementRootNamespace))
            {
                serializer = new XmlSerializer(objectType, new XmlRootAttribute(rootElementName) { Namespace = elementRootNamespace });
            }
            else
            {
                serializer = new XmlSerializer(objectType, new XmlRootAttribute(rootElementName));
            }

            return serializer;
        }
    }
}
