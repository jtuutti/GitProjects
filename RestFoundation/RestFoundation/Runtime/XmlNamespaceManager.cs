// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Xml;
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

        public static void AddNamespaces(XmlDocument document)
        {
            string serviceNamespace = Rest.Configuration.Options.XmlSettings != null && !String.IsNullOrWhiteSpace(Rest.Configuration.Options.XmlSettings.Namespace) ?
                                      Rest.Configuration.Options.XmlSettings.Namespace :
                                      null;

            if (String.IsNullOrWhiteSpace(serviceNamespace))
            {
                return;
            }

            XmlElement documentElement = document.DocumentElement;

            if (documentElement == null)
            {
                return;
            }

            documentElement.SetAttribute("xmlns", serviceNamespace);
        }
    }
}
