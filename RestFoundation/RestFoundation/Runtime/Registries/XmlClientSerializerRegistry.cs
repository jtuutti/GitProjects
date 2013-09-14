// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RestFoundation.Runtime
{
    internal static class XmlSerializerRegistry
    {
        private static readonly ConcurrentDictionary<Type, XmlSerializer> serializers = new ConcurrentDictionary<Type, XmlSerializer>();
        private static readonly object syncRoot = new Object();

        public static XmlSerializer Get(Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType");
            }

            return serializers.GetOrAdd(objectType, InitializeXmlSerializer);
        }

        private static XmlSerializer InitializeXmlSerializer(Type objectType)
        {
            lock (syncRoot)
            {
                if (objectType.IsArray)
                {
                    return GetSerializerForCollection(objectType, objectType.GetElementType());
                }

                if (objectType.IsGenericType && objectType.GetGenericTypeDefinition().GetInterface(typeof(IEnumerable<>).FullName) != null)
                {
                    return GetSerializerForCollection(objectType, objectType.GetGenericArguments()[0]);
                }

                string rootNamespace = XmlNamespaceManager.GetDefault();

                return !String.IsNullOrWhiteSpace(rootNamespace) ? new XmlSerializer(objectType, rootNamespace) : new XmlSerializer(objectType);
            }
        }

        private static XmlSerializer GetSerializerForCollection(Type objectType, Type elementType)
        {
            string rootNamespace;
            string rootElementName = XmlRootElementInspector.GetRootElementName(elementType, out rootNamespace);

            if (String.IsNullOrWhiteSpace(rootNamespace))
            {
                rootNamespace = XmlNamespaceManager.GetDefault();
            }

            XmlSerializer serializer;

            if (!String.IsNullOrWhiteSpace(rootNamespace))
            {
                serializer = new XmlSerializer(objectType, new XmlRootAttribute(rootElementName) { Namespace = rootNamespace });
            }
            else
            {
                serializer = new XmlSerializer(objectType, new XmlRootAttribute(rootElementName));
            }

            return serializer;
        }
    }
}
