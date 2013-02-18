// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RestFoundation.Runtime;

namespace RestFoundation.Client.Serializers
{
    /// <summary>
    /// Represents an XML serializer.
    /// </summary>
    public class XmlObjectSerializer : IRestSerializer
    {
        private readonly string m_xmlNamespace;
        private string m_serializedObject;
        private object m_reference;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlObjectSerializer"/> class.
        /// </summary>
        /// <param name="xmlNamespace">An XML namespace.</param>
        public XmlObjectSerializer(string xmlNamespace)
        {
            m_xmlNamespace = xmlNamespace;
        }

        /// <summary>
        /// Gets content length of an object in the serialized form.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A value containing the serialized object length.</returns>
        public int GetContentLength(object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            if (m_serializedObject != null && ReferenceEquals(obj, m_reference))
            {
                return m_serializedObject.Length;
            }

            m_reference = obj;

            m_serializedObject = SerializeToString(obj);
            return m_serializedObject.Length;
        }

        /// <summary>
        /// Serializes an object into a stream.
        /// </summary>
        /// <param name="stream">The output stream.</param>
        /// <param name="obj">The object to serialize.</param>
        public void Serialize(Stream stream, object obj)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (obj == null)
            {
                return;
            }

            if (m_serializedObject == null || !ReferenceEquals(obj, m_reference))
            {
                m_serializedObject = SerializeToString(obj);
            }

            byte[] data = Encoding.UTF8.GetBytes(m_serializedObject);
            stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Deserializes an object of the provided type from the stream.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="stream">The input stream.</param>
        /// <returns>The deserialized object.</returns>
        public T Deserialize<T>(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            Type objectType = typeof(T);

            if (objectType == typeof(object))
            {
                using (var streamReader = new StreamReader(stream, Encoding.UTF8))
                {
                    string xmlData = streamReader.ReadToEnd();

                    if (String.IsNullOrWhiteSpace(xmlData))
                    {
                        return default(T);
                    }

                    return (dynamic) new DynamicXDocument(xmlData);
                }
            }

            XmlSerializer serializer = XmlClientSerializerRegistry.Get(objectType, m_xmlNamespace);
            return (T) serializer.Deserialize(stream);
        }

        private string SerializeToString(object obj)
        {
            using (var stream = new MemoryStream())
            {
                XmlSerializer serializer = XmlClientSerializerRegistry.Get(obj.GetType(), m_xmlNamespace);
                serializer.Serialize(stream, obj);

                stream.Position = 0;

                var reader = new StreamReader(stream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }
    }
}
