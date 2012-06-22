using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace RestFoundation.Runtime
{
    public class StreamCompressor : IStreamCompressor
    {
        private const string Deflate = "deflate";
        private const string Gzip = "gzip";
        private const string XDeflate = "x-deflate";
        private const string XGzip = "x-gzip";
        private const string AllEncodings = "*";

        public Stream Compress(Stream output, IEnumerable<string> acceptedEncodings, out string outputEncoding)
        {
            if (output == null || acceptedEncodings == null || output is DeflateStream || output is GZipStream)
            {
                outputEncoding = null;
                return output;
            }

            foreach (var compressionEncoding in acceptedEncodings)
            {
                if (String.Equals(Deflate, compressionEncoding, StringComparison.OrdinalIgnoreCase) ||
                    String.Equals(XDeflate, compressionEncoding, StringComparison.OrdinalIgnoreCase))
                {
                    outputEncoding = Deflate;
                    return new DeflateStream(output, CompressionMode.Compress);
                }

                if (String.Equals(Gzip, compressionEncoding, StringComparison.OrdinalIgnoreCase) ||
                    String.Equals(XGzip, compressionEncoding, StringComparison.OrdinalIgnoreCase) ||
                    String.Equals(AllEncodings, compressionEncoding))
                {
                    outputEncoding = Gzip;
                    return new GZipStream(output, CompressionMode.Compress);
                }
            }

            outputEncoding = null;
            return output;
        }
    }
}
