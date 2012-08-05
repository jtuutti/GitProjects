// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System.IO;

namespace RestFoundation.Client
{
    /// <summary>
    /// Defines a REST client serializer.
    /// </summary>
    public interface IRestSerializer
    {
        /// <summary>
        /// Gets content length of an object in the serialized form.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A value containing the serialized object length.</returns>
        int GetContentLength(object obj);

        /// <summary>
        /// Serializes an object into a stream.
        /// </summary>
        /// <param name="stream">The output stream.</param>
        /// <param name="obj">The object to serialize.</param>
        void Serialize(Stream stream, object obj);

        /// <summary>
        /// Deserializes an object of the provided type from the stream.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="stream">The input stream.</param>
        /// <returns>The deserialized object.</returns>
        T Deserialize<T>(Stream stream);
    }
}
