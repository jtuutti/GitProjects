// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using RestFoundation.Runtime;

namespace RestFoundation.Client.Serializers
{
    /// <summary>
    /// Represents a REST client XML serializer.
    /// </summary>
    public class XmlClientSerializer : IRestClientSerializer
    {
        private readonly string m_xmlNamespace;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlClientSerializer"/> class.
        /// </summary>
        /// <param name="xmlNamespace">An XML namespace.</param>
        public XmlClientSerializer(string xmlNamespace)
        {
            m_xmlNamespace = xmlNamespace;
        }

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

            string serializedObject = await SerializeToString(obj);

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
        public async Task<T> DeserializeAsync<T>(Stream stream)
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
                    string xmlData = await streamReader.ReadToEndAsync();

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

        private Task<string> SerializeToString(object obj)
        {
            using (var stream = new MemoryStream())
            {
                XmlSerializer serializer = XmlClientSerializerRegistry.Get(obj.GetType(), m_xmlNamespace);
                serializer.Serialize(stream, obj);

                stream.Position = 0;

                var reader = new StreamReader(stream, Encoding.UTF8);
                return reader.ReadToEndAsync();
            }
        }
    }
}
