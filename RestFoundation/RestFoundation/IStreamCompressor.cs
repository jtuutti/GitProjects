// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System.Collections.Generic;
using System.IO;

namespace RestFoundation
{
    /// <summary>
    /// Defines a stream compressor.
    /// </summary>
    public interface IStreamCompressor
    {
        /// <summary>
        /// Returns a stream instance that compresses the data of the original output stream along
        /// with the chosen output encoding.
        /// </summary>
        /// <param name="output">The output stream to compress.</param>
        /// <param name="acceptedEncodings">A sequence of accepted encodings.</param>
        /// <param name="outputEncoding">The chosen output encoding.</param>
        /// <returns>The compressed stream.</returns>
        Stream Compress(Stream output, IEnumerable<string> acceptedEncodings, out string outputEncoding);
    }
}
