using System.Collections.Generic;
using System.IO;

namespace RestFoundation
{
    public interface IStreamCompressor
    {
        Stream Compress(Stream output, IEnumerable<string> acceptedEncodings, out string outputEncoding);
    }
}
