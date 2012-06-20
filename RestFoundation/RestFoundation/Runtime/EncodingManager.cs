using System;
using Ionic.Zlib;

namespace RestFoundation.Runtime
{
    internal static class EncodingManager
    {
        private const string Deflate = "deflate";
        private const string Gzip = "gzip";
        private const string XDeflate = "x-deflate";
        private const string XGzip = "x-gzip";
        private const string ContentEncodingHeader = "Content-Encoding";

        public static void FilterResponse(IHttpRequest request, IHttpResponse response)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (response == null) throw new ArgumentNullException("response");

            if (response.OutputFilter is GZipStream || response.OutputFilter is DeflateStream ||
                !String.IsNullOrEmpty(response.GetHeader(ContentEncodingHeader)))
            {
                return;
            }

            foreach (var compressionEncoding in request.Headers.AcceptEncodings)
            {
                if (String.Equals(Gzip, compressionEncoding, StringComparison.OrdinalIgnoreCase) ||
                    String.Equals(XGzip, compressionEncoding, StringComparison.OrdinalIgnoreCase))
                {
                    response.SetHeader(ContentEncodingHeader, Gzip);
                    response.OutputFilter = new GZipStream(response.OutputFilter, CompressionMode.Compress);
                    break;
                }

                if (String.Equals(Deflate, compressionEncoding, StringComparison.OrdinalIgnoreCase) ||
                    String.Equals(XDeflate, compressionEncoding, StringComparison.OrdinalIgnoreCase))
                {
                    response.SetHeader(ContentEncodingHeader, Deflate);
                    response.OutputFilter = new DeflateStream(response.OutputFilter, CompressionMode.Compress);
                    break;
                }
            }
        }
    }
}
