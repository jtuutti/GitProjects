// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Xml.Serialization;

namespace RestFoundation.Runtime
{
    internal static class XmlNamespaceManager
    {
        public static XmlSerializerNamespaces Generate()
        {
            if (Rest.Configuration.Options.XmlSettings != null && !String.IsNullOrWhiteSpace(Rest.Configuration.Options.XmlSettings.Namespace))
            {
                return null;
            }

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            return namespaces;
        }

        public static string GetDefault()
        {
            if (Rest.Configuration.Options.XmlSettings == null || String.IsNullOrWhiteSpace(Rest.Configuration.Options.XmlSettings.Namespace))
            {
                return null;
            }

            return Rest.Configuration.Options.XmlSettings.Namespace;
        }
    }
}
