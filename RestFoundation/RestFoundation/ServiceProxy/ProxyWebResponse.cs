// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.Serialization;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a web response class for the service proxy.
    /// </summary>
    [Serializable]
    public sealed class ProxyWebResponse : WebResponse
    {
        private const string ContentEncodingHeader = "Content-Encoding";

        private readonly HttpWebResponse m_httpResponse;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyWebResponse"/> class.
        /// </summary>
        /// <param name="response">The web response.</param>
        public ProxyWebResponse(WebResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            var webResponse = response as HttpWebResponse;

            if (webResponse == null)
            {
                throw new ArgumentException(RestResources.InvalidWebResponse, "response");
            }

            m_httpResponse = webResponse;
        }

        private ProxyWebResponse(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
            var httpResponse = serializationInfo.GetValue("m_httpResponse", typeof(HttpWebResponse)) as HttpWebResponse;

            if (httpResponse != null)
            {
                m_httpResponse = httpResponse;
            }
        }

        /// <summary>
        /// Gets the HTTP protocol version.
        /// </summary>
        public Version ProtocolVersion
        {
            get
            {
                return m_httpResponse.ProtocolVersion;
            }
        }

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get
            {
                return m_httpResponse.StatusCode;
            }
        }

        /// <summary>
        /// Gets the HTTP status description.
        /// </summary>
        public string StatusDescription
        {
            get
            {
                return m_httpResponse.StatusDescription;
            }
        }

        /// <summary>
        /// Gets or sets the content length of data being received.
        /// </summary>
        /// <returns>
        /// The number of bytes returned from the Internet resource.
        /// </returns>
        public override long ContentLength
        {
            get
            {
                return m_httpResponse.ContentLength;
            }
            set
            {
                m_httpResponse.ContentLength = value;
            }
        }

        /// <summary>
        /// Gets or sets the content type of the data being received.
        /// </summary>
        /// <returns>
        /// A string that contains the content type of the response.
        /// </returns>
        public override string ContentType
        {
            get
            {
                return m_httpResponse.ContentType;
            }
            set
            {
                m_httpResponse.ContentType = value;
            }
        }

        /// <summary>
        /// Gets a collection of header name-value pairs associated with this request.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="WebHeaderCollection"/> class that contains header values associated with this response.
        /// </returns>
        public override WebHeaderCollection Headers
        {
            get
            {
                return m_httpResponse.Headers ?? new WebHeaderCollection();
            }
        }

        /// <summary>
        /// Gets a <see cref="T:System.Boolean"/> value that indicates whether this response was obtained from the cache.
        /// </summary>
        /// <returns>
        /// true if the response was taken from the cache; otherwise, false.
        /// </returns>
        public override bool IsFromCache
        {
            get
            {
                return m_httpResponse.IsFromCache;
            }
        }

        /// <summary>
        /// Gets a <see cref="T:System.Boolean"/> value that indicates whether mutual authentication occurred.
        /// </summary>
        /// <returns>
        /// true if both client and server were authenticated; otherwise, false.
        /// </returns>
        public override bool IsMutuallyAuthenticated
        {
            get
            {
                return m_httpResponse.IsMutuallyAuthenticated;
            }
        }

        /// <summary>
        /// Gets the URI of the Internet resource that actually responded to the request.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="Uri"/> class that contains the URI of the Internet resource that actually responded to the request.
        /// </returns>
        public override Uri ResponseUri
        {
            get
            {
                return m_httpResponse.ResponseUri;
            }
        }

        /// <summary>
        /// Returns the data stream from the Internet resource.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="Stream"/> class for reading data from the Internet resource.
        /// </returns>
        public override Stream GetResponseStream()
        {
            Stream responseStream = m_httpResponse.GetResponseStream();

            if (responseStream == null)
            {
                return null;
            }

            string encoding = Headers[ContentEncodingHeader];

            if (String.Equals("gzip", encoding, StringComparison.OrdinalIgnoreCase))
            {
                return new GZipStream(responseStream, CompressionMode.Decompress);
            }

            if (String.Equals("deflate", encoding, StringComparison.OrdinalIgnoreCase))
            {
                return new DeflateStream(responseStream, CompressionMode.Decompress);
            }

            return responseStream;
        }

        /// <summary>
        /// Closes the response stream.
        /// </summary>
        public override void Close()
        {
            m_httpResponse.Close();
        }

        /// <summary>
        /// Populates a <see cref="SerializationInfo"/> with the data that is needed to serialize the target object.
        /// </summary>
        /// <param name="serializationInfo">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="streamingContext">A <see cref="StreamingContext"/> that specifies the destination for this serialization.</param>
        protected override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            if (serializationInfo == null)
            {
                throw new ArgumentNullException("serializationInfo");
            }

            base.GetObjectData(serializationInfo, streamingContext);

            serializationInfo.AddValue("m_httpResponse", m_httpResponse, typeof(HttpWebResponse));
        }
    }
}
