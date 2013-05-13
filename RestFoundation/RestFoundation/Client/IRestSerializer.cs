// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace RestFoundation.Client
{
    /// <summary>
    /// Defines a REST client serializer.
    /// </summary>
    public interface IRestSerializer
    {
        /// <summary>
        /// Serializes an object into a stream.
        /// </summary>
        /// <param name="request">The web request to update.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The serialization task.</returns>
        Task SerializeAsync(WebRequest request, object obj);

        /// <summary>
        /// Deserializes an object of the provided type from the stream.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="stream">The input stream.</param>
        /// <returns>The deserialization task.</returns>
        Task<T> DeserializeAsync<T>(Stream stream);
    }
}
