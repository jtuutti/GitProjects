// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.IO;
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
        private string m_serializedObject;
        private object m_reference;

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

            if (typeof(T) == typeof(object))
            {
                using (var streamReader = new StreamReader(stream, Encoding.UTF8))
                {
                    return (dynamic) new DynamicXDocument(streamReader.ReadToEnd());
                }
            }

            var serializer = new XmlSerializer(typeof(T));
            return (T) serializer.Deserialize(stream);
        }

        private static string SerializeToString(object obj)
        {
            using (var stream = new MemoryStream())
            {
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(String.Empty, String.Empty);

                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(stream, obj, namespaces);
                stream.Position = 0;

                var reader = new StreamReader(stream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }
    }
}
