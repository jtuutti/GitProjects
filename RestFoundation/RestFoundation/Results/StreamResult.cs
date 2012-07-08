using System;
using System.IO;
using System.Net;
using RestFoundation.Context;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    public class StreamResult : IResult
    {
        public StreamResult()
        {
            ClearOutput = true;
        }

        public Stream Stream { get; set; }
        public string ContentType { get; set; }
        public string ContentDisposition { get; set; }
        public bool ClearOutput { get; set; }

        public virtual void Execute(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (Stream == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No valid input stream provided");
            }

            if (ClearOutput)
            {
                context.Response.Output.Clear();
            }

            if (!String.IsNullOrEmpty(ContentType))
            {
                context.Response.SetHeader(context.Response.Headers.ContentType, ContentType);
            }
            else
            {
                string acceptType = context.Request.GetPreferredAcceptType();

                if (!String.IsNullOrEmpty(acceptType))
                {
                    context.Response.SetHeader(context.Response.Headers.ContentType, acceptType);
                }
            }

            if (!String.IsNullOrEmpty(ContentDisposition))
            {
                context.Response.SetHeader(context.Response.Headers.ContentDisposition, ContentType);
            }

            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);

            OutputCompressionManager.FilterResponse(context);

            using (Stream)
            {
                if (Stream.CanSeek)
                {
                    Stream.Seek(0, SeekOrigin.Begin);
                }

                Stream.CopyTo(context.Response.Output.Stream);
            }
        }
    }
}
