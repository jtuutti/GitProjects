// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestFoundation.Runtime;

namespace RestFoundation.Client.Serializers
{
    /// <summary>
    /// Represents a JSON serializer.
    /// </summary>
    public class JsonObjectSerializer : IRestSerializer
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

            string serializedObject = JsonConvert.SerializeObject(obj, Rest.Configuration.Options.JsonSettings.ToJsonSerializerSettings());

            byte[] data = Encoding.UTF8.GetBytes(serializedObject);
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
        public Task<T> DeserializeAsync<T>(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                var serializer = JsonSerializerFactory.Create();
                var reader = new JsonTextReader(streamReader);

                if (typeof(T) == typeof(object))
                {
                    dynamic deserializedObject = serializer.Deserialize(reader);
                    return Task.FromResult<dynamic>(deserializedObject);
                }

                return Task.FromResult(serializer.Deserialize<T>(reader));
            }
        }
    }
}
