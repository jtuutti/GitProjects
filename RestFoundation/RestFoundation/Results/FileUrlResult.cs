using System;
using System.IO;
using System.Net;
using RestFoundation.Context;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    public class FileUrlResult : IResult
    {
        public FileUrlResult()
        {
            ClearOutput = true;
        }

        public string FileUrl { get; set; }
        public string ContentType { get; set; }
        public string ContentDisposition { get; set; }
        public TimeSpan CacheForTimeSpan { get; set; }
        public bool ClearOutput { get; set; }

        public virtual void Execute(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (FileUrl == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No valid file URL provided");
            }

            var file = new FileInfo(context.MapPath(FileUrl));

            if (!file.Exists)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No valid file URL provided");
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

            if (CacheForTimeSpan > TimeSpan.Zero)
            {
                context.Response.SetFileDependencies(file.FullName, CacheForTimeSpan);
            }

            context.Response.TransmitFile(file.FullName);
        }
    }
}
