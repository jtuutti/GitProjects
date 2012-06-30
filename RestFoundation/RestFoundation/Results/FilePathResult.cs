using System;
using System.IO;
using System.Net;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    public class FilePathResult : IResult
    {
        public FilePathResult()
        {
            ClearOutput = true;
        }

        public string FilePath { get; set; }
        public string ContentType { get; set; }
        public string ContentDisposition { get; set; }
        public bool CacheOutput { get; set; }
        public bool ClearOutput { get; set; }

        public void Execute(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (FilePath == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No valid file path provided");
            }

            var file = new FileInfo(FilePath);

            if (!file.Exists)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No valid file path provided");
            }

            if (ClearOutput)
            {
                context.Response.Output.Clear();
            }

            if (!String.IsNullOrEmpty(ContentType))
            {
                context.Response.SetHeader("Content-Type", ContentType);
            }
            else
            {
                string acceptType = context.Request.GetPreferredAcceptType();

                if (!String.IsNullOrEmpty(acceptType))
                {
                    context.Response.SetHeader("Content-Type", acceptType);
                }
            }

            if (!String.IsNullOrEmpty(ContentDisposition))
            {
                context.Response.SetHeader("Content-Disposition", ContentType);
            }

            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);

            OutputCompressionManager.FilterResponse(context);

            if (CacheOutput)
            {
                context.Response.SetFileDependencies(file.FullName);
            }

            context.Response.TransmitFile(file.FullName);
        }
    }
}
