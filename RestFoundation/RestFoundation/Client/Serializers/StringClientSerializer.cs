// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestFoundation.Client.Serializers
{
    /// <summary>
    /// Represents a REST client <see cref="String"/> serializer.
    /// </summary>
    public class StringClientSerializer : IRestClientSerializer
    {
        /// <summary>
        /// Serializes an object into a stream.
        /// </summary>
        /// <param name="request">The web request to update.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The serialization task.</returns>
        public async Task SerializeAsync(WebRequest request, object obj)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (obj == null)
            {
                return;
            }

            if (!(obj is string))
            {
                throw new ArgumentOutOfRangeException("obj");
            }

            byte[] data = Encoding.UTF8.GetBytes(obj.ToString());
            request.ContentLength = data.LongLength;

            Stream requestStream = await Task<Stream>.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, request).ConfigureAwait(false);
            await requestStream.WriteAsync(data, 0, data.Length);
        }

        /// <summary>
        /// Deserializes an object of the provided type from the stream.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="stream">The input stream.</param>
        /// <returns>The deserialization task.</returns>
        public async Task<T> DeserializeAsync<T>(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (typeof(T) != typeof(string))
            {
                throw new InvalidOperationException(Resources.Global.NonStringOutputType);
            }

            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                object serializedObject = await reader.ReadToEndAsync();
                return (T) serializedObject;
            }
        }
    }
}
