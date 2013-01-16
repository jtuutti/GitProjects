// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using RestFoundation.Runtime;
using XmlNamespaceManager = RestFoundation.Runtime.XmlNamespaceManager;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents an XML object converter.
    /// </summary>
    public static class ProxyXmlConvert
    {
        /// <summary>
        /// Converts an object into XML with the provided formatting options.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="isFormatted">
        /// A <see cref="bool"/> indicating whether the serialized object is formatted for output.
        /// </param>
        /// <returns>The <see cref="String"/> value containing the serialized object.</returns>
        public static string SerializeObject(object obj, bool isFormatted)
        {
            if (obj == null)
            {
                return String.Empty;
            }

            var serializer = XmlSerializerRegistry.Get(obj.GetType());

            using (var stream = new MemoryStream())
            {
                var xmlWriter = new XmlTextWriter(stream, Encoding.UTF8)
                                {
                                    Formatting = isFormatted ? Formatting.Indented : Formatting.None
                                };

                serializer.Serialize(xmlWriter, obj, XmlNamespaceManager.Generate());
                xmlWriter.Flush();

                stream.Seek(0, SeekOrigin.Begin);

                var xmlReader = new StreamReader(stream, Encoding.UTF8);
                return xmlReader.ReadToEnd();
            }
        }
    }
}
