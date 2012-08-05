// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace RestFoundation.Client.Serializers
{
    /// <summary>
    /// Represents a JSON serializer.
    /// </summary>
    public class JsonObjectSerializer : IRestSerializer
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

            m_serializedObject = JsonConvert.SerializeObject(obj, Formatting.None);
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
                m_serializedObject = JsonConvert.SerializeObject(obj, Formatting.None);    
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

            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(streamReader);

                if (typeof(T) == typeof(object))
                {
                    dynamic obj = serializer.Deserialize(reader);
                    return obj;
                }

                return serializer.Deserialize<T>(reader);
            }
        }
    }
}
