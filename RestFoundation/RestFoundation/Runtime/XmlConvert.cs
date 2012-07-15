using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents an XML object converter.
    /// </summary>
    public static class XmlConvert
    {
        /// <summary>
        /// Converts an object into XML.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The <see cref="String"/> value containing the serialized object.</returns>
        public static string SerializeObject(object obj)
        {
            return SerializeObject(obj, Formatting.None);
        }

        /// <summary>
        /// Converts an object into XML with the provided formatting options.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="formatting">The object containing XML serialization formatting options.</param>
        /// <returns>The <see cref="String"/> value containing the serialized object.</returns>
        public static string SerializeObject(object obj, Formatting formatting)
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
                                    Formatting = formatting
                                };

                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(String.Empty, String.Empty);

                serializer.Serialize(xmlWriter, obj, namespaces);
                xmlWriter.Flush();

                stream.Seek(0, SeekOrigin.Begin);

                var xmlReader = new StreamReader(stream, Encoding.UTF8);
                return xmlReader.ReadToEnd();
            }
        }
    }
}
