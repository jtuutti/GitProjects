using System;
using System.Collections.Concurrent;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
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

            return serializers.GetOrAdd(objectType, type  => type.IsArray ? GetSerializerForArray(type) : new XmlSerializer(type));
        }

        private static XmlSerializer GetSerializerForArray(Type objectType)
        {
            XmlSerializer serializer;

            Type elementType = objectType.GetElementType();
            var xmlRootAttribute = elementType.GetCustomAttributes(typeof(XmlRootAttribute), true).OfType<XmlRootAttribute>().FirstOrDefault();

            string elementName = xmlRootAttribute != null && !String.IsNullOrEmpty(xmlRootAttribute.ElementName) ? xmlRootAttribute.ElementName : elementType.Name;

            var pluralization = PluralizationService.CreateService(CultureInfo.CurrentCulture);

            if (!pluralization.IsPlural(elementName))
            {
                elementName = pluralization.Pluralize(elementName);
            }

            if (xmlRootAttribute != null && !String.IsNullOrEmpty(xmlRootAttribute.Namespace))
            {
                serializer = new XmlSerializer(objectType, new XmlRootAttribute(elementName) { Namespace = xmlRootAttribute.Namespace });
            }
            else
            {
                serializer = new XmlSerializer(objectType, new XmlRootAttribute(elementName));
            }

            return serializer;
        }
    }
}
