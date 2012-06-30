using System;

namespace RestFoundation.Runtime
{
    internal static class OutputCompressionManager
    {
        private const string ContentEncodingHeader = "Content-Encoding";

        public static void FilterResponse(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var streamCompressor = Rest.Active.CreateObject<IStreamCompressor>();

            if (streamCompressor == null)
            {
                return;
            }

            string outputEncoding;
            context.Response.Output.Filter = streamCompressor.Compress(context.Response.Output.Filter, context.Request.Headers.AcceptEncodings, out outputEncoding);

            if (!String.IsNullOrEmpty(outputEncoding))
            {
                context.Response.SetHeader(ContentEncodingHeader, outputEncoding);
            }
        }
    }
}
