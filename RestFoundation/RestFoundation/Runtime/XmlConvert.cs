using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RestFoundation.Runtime
{
    public static class XmlConvert
    {
        public static string SerializeObject(object obj)
        {
            return SerializeObject(obj, Formatting.None);
        }

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
