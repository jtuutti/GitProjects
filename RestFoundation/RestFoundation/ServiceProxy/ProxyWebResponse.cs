using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace RestFoundation.ServiceProxy
{
    public sealed class ProxyWebResponse : WebResponse
    {
        private const string ContentEncodingHeader = "Content-Encoding";

        private readonly HttpWebResponse httpResponse;

        public ProxyWebResponse(WebResponse response)
        {
            if (response == null) throw new ArgumentNullException("response");

            var webResponse = response as HttpWebResponse;
            if (webResponse == null) throw new ArgumentException("Invalid web response provided.", "response");

            httpResponse = webResponse;
        }

        public Version ProtocolVersion
        {
            get
            {
                return httpResponse.ProtocolVersion;
            }
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                return httpResponse.StatusCode;
            }
        }

        public string StatusDescription
        {
            get
            {
                return httpResponse.StatusDescription;
            }
        }

        public override long ContentLength
        {
            get
            {
                return httpResponse.ContentLength;
            }
            set
            {
                httpResponse.ContentLength = value;
            }
        }

        public override string ContentType
        {
            get
            {
                return httpResponse.ContentType;
            }
            set
            {
                httpResponse.ContentType = value;
            }
        }

        public override WebHeaderCollection Headers
        {
            get
            {
                return httpResponse.Headers ?? new WebHeaderCollection();
            }
        }

        public override bool IsFromCache
        {
            get
            {
                return httpResponse.IsFromCache;
            }
        }

        public override bool IsMutuallyAuthenticated
        {
            get
            {
                return httpResponse.IsMutuallyAuthenticated;
            }
        }

        public override Uri ResponseUri
        {
            get
            {
                return httpResponse.ResponseUri;
            }
        }

        public override Stream GetResponseStream()
        {
            Stream responseStream = httpResponse.GetResponseStream();
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
            httpResponse.Close();
        }
    }
}
