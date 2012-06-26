using System;

namespace RestFoundation.Runtime
{
    internal static class OutputCompressionManager
    {
        private const string ContentEncodingHeader = "Content-Encoding";

        public static void FilterResponse(IHttpRequest request, IHttpResponse response)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (response == null) throw new ArgumentNullException("response");

            var streamCompressor = Rest.Active.CreateObject<IStreamCompressor>();

            if (streamCompressor == null)
            {
                return;
            }

            string outputEncoding;
            response.Output.Filter = streamCompressor.Compress(response.Output.Filter, request.Headers.AcceptEncodings, out outputEncoding);

            if (!String.IsNullOrEmpty(outputEncoding))
            {
                response.SetHeader(ContentEncodingHeader, outputEncoding);
            }
        }
    }
}
