// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents the default stream compressor that supports GZip and Deflate.
    /// </summary>
    public class StreamCompressor : IStreamCompressor
    {
        private const string Deflate = "deflate";
        private const string Gzip = "gzip";
        private const string XDeflate = "x-deflate";
        private const string XGzip = "x-gzip";
        private const string AllEncodings = "*";

        /// <summary>
        /// Returns a stream instance that compresses the data of the original output stream along
        /// with the chosen output encoding.
        /// </summary>
        /// <param name="output">The output stream to compress.</param>
        /// <param name="acceptedEncodings">A sequence of accepted encodings.</param>
        /// <param name="outputEncoding">The chosen output encoding.</param>
        /// <returns>The compressed stream.</returns>
        public virtual Stream Compress(Stream output, IEnumerable<string> acceptedEncodings, out string outputEncoding)
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
