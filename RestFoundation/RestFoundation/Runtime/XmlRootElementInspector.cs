// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Reflection;
using System.Xml.Serialization;

namespace RestFoundation.Runtime
{
    internal static class XmlRootElementInspector
    {
        public static string GetRootElementName(Type objectType, out string rootNamespace)
        {           
            var xmlRootAttribute = objectType.GetCustomAttribute<XmlRootAttribute>(true);
            string elementName;

            if (xmlRootAttribute != null)
            {
                elementName = !String.IsNullOrEmpty(xmlRootAttribute.ElementName) ? xmlRootAttribute.ElementName : objectType.Name;
                rootNamespace = xmlRootAttribute.Namespace;
            }
            else
            {
                elementName = objectType.Name;
                rootNamespace = null;
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
