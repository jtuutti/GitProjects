// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Xml.Serialization;

namespace RestFoundation.Runtime
{
    internal static class XmlRootElementInspector
    {
        public static string GetRootElementName(Type objectType, out string rootNameSpace)
        {           
            var xmlRootAttribute = Attribute.GetCustomAttribute(objectType, typeof(XmlRootAttribute), true) as XmlRootAttribute;

            string elementName;

            if (xmlRootAttribute != null && !String.IsNullOrEmpty(xmlRootAttribute.ElementName))
            {
                elementName = xmlRootAttribute.ElementName;
                rootNameSpace = xmlRootAttribute.Namespace;
            }
            else
            {
                elementName = objectType.Name;
                rootNameSpace = null;
            }

            var pluralization = PluralizationService.CreateService(CultureInfo.CurrentCulture);

            if (!pluralization.IsPlural(elementName))
            {
                elementName = pluralization.Pluralize(elementName);
            }

            return elementName;
        }
    }
}
