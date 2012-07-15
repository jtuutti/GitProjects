using System;
using System.IO;
using System.Text;

namespace RestFoundation.Client.Serializers
{
    /// <summary>
    /// Represents a <see cref="String"/> serializer.
    /// </summary>
    public class StringSerializer : IRestSerializer
    {
        /// <summary>
        /// Gets content length of an object in the serialized form.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A value containing the serialized object length.</returns>
        public int GetContentLength(object obj)
        {
            if (obj == null) return 0;
            if (!(obj is String)) throw new ArgumentOutOfRangeException("obj");

            return obj.ToString().Length;
        }

        /// <summary>
        /// Serializes an object into a stream.
        /// </summary>
        /// <param name="stream">The output stream.</param>
        /// <param name="obj">The object to serialize.</param>
        public void Serialize(Stream stream, object obj)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (obj == null) return;
            if (!(obj is String)) throw new ArgumentOutOfRangeException("obj");

            byte[] data = Encoding.UTF8.GetBytes(obj.ToString());
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
            if (stream == null) throw new ArgumentNullException("stream");
            if (typeof(T) != typeof(String)) throw new InvalidOperationException("The output type is not a String.");

            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                object serializedObject = reader.ReadToEnd();
                return (T) serializedObject;
            }
        }
    }
}
