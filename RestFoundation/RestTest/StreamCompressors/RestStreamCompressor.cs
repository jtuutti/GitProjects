using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zlib;
using RestFoundation;

namespace RestTest.StreamCompressors
{
    public class RestStreamCompressor : IStreamCompressor
    {
        public Stream Compress(Stream output, IEnumerable<string> acceptedEncodings, out string outputEncoding)
        {
            if (output == null || acceptedEncodings == null || output is DeflateStream || output is GZipStream)
            {
                outputEncoding = null;
                return output;
            }

            foreach (var compressionEncoding in acceptedEncodings)
            {
                if (String.Equals("deflate", compressionEncoding, StringComparison.OrdinalIgnoreCase))
                {
                    outputEncoding = compressionEncoding.ToLowerInvariant();
                    return new DeflateStream(output, CompressionMode.Compress);
                }

                if (String.Equals("gzip", compressionEncoding, StringComparison.OrdinalIgnoreCase) ||
                    String.Equals("*", compressionEncoding))
                {
                    outputEncoding = compressionEncoding.ToLowerInvariant();
                    return new GZipStream(output, CompressionMode.Compress);
                }
            }

            outputEncoding = null;
            return output;
        }
    }
}