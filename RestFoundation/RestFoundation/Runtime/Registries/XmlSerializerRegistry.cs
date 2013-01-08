﻿// <copyright>
// Dmitry Starosta, 2012
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
            if (objectType.IsArray)
            {
                return GetSerializerForCollection(objectType, objectType.GetElementType());
            }

            if (objectType.IsGenericType && objectType.GetGenericTypeDefinition().GetInterface(typeof(IEnumerable<>).FullName) != null)
            {
                return GetSerializerForCollection(objectType, objectType.GetGenericArguments()[0]);
            }

            string rootNameSpace = XmlNameSpaceExtractor.Get();

            return !String.IsNullOrWhiteSpace(rootNameSpace) ? new XmlSerializer(objectType, rootNameSpace) : new XmlSerializer(objectType);
        }

        private static XmlSerializer GetSerializerForCollection(Type objectType, Type elementType)
        {
            string rootNameSpace;
            string rootElementName = XmlRootElementInspector.GetRootElementName(elementType, out rootNameSpace);

            if (String.IsNullOrWhiteSpace(rootNameSpace))
            {
                rootNameSpace = XmlNameSpaceExtractor.Get();
            }

            XmlSerializer serializer;

            if (!String.IsNullOrEmpty(rootNameSpace))
            {
                serializer = new XmlSerializer(objectType, new XmlRootAttribute(rootElementName) { Namespace = rootNameSpace });
            }
            else
            {
                serializer = new XmlSerializer(objectType, new XmlRootAttribute(rootElementName));
            }

            return serializer;
        }
    }
}
