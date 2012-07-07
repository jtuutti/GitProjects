using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.Serialization;

namespace RestFoundation.ServiceProxy
{
    [Serializable]
    public sealed class ProxyWebResponse : WebResponse
    {
        private const string ContentEncodingHeader = "Content-Encoding";

        private readonly HttpWebResponse m_httpResponse;

        public ProxyWebResponse(WebResponse response)
        {
            if (response == null) throw new ArgumentNullException("response");

            var webResponse = response as HttpWebResponse;
            if (webResponse == null) throw new ArgumentException("Invalid web response provided.", "response");

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

        public Version ProtocolVersion
        {
            get
            {
                return m_httpResponse.ProtocolVersion;
            }
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                return m_httpResponse.StatusCode;
            }
        }

        public string StatusDescription
        {
            get
            {
                return m_httpResponse.StatusDescription;
            }
        }

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

        public override WebHeaderCollection Headers
        {
            get
            {
                return m_httpResponse.Headers ?? new WebHeaderCollection();
            }
        }

        public override bool IsFromCache
        {
            get
            {
                return m_httpResponse.IsFromCache;
            }
        }

        public override bool IsMutuallyAuthenticated
        {
            get
            {
                return m_httpResponse.IsMutuallyAuthenticated;
            }
        }

        public override Uri ResponseUri
        {
            get
            {
                return m_httpResponse.ResponseUri;
            }
        }

        public override Stream GetResponseStream()
        {
            Stream responseStream = m_httpResponse.GetResponseStream();
            if (responseStream == null) return null;

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

        public override void Close()
        {
            m_httpResponse.Close();
        }

        protected override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            if (serializationInfo == null) throw new ArgumentNullException("serializationInfo");

            base.GetObjectData(serializationInfo, streamingContext);

            serializationInfo.AddValue("m_httpResponse", m_httpResponse, typeof(HttpWebResponse));
        }
    }
}
