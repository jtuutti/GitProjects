using System;
using System.IO;
using System.Net;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    public class StreamResult : IResult
    {
        public StreamResult()
        {
            ClearResponse = true;
        }

        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }
        public Stream Stream { get; set; }
        public string ContentType { get; set; }
        public string ContentDisposition { get; set; }
        public bool ClearResponse { get; set; }

        public void Execute()
        {
            if (Response == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No HTTP context found");
            }

            if (Stream == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No valid input stream provided");
            }

            if (ClearResponse)
            {
                Response.Clear();
            }

            if (!String.IsNullOrEmpty(ContentType))
            {
                Response.SetHeader("Content-Type", ContentType);
            }

            if (!String.IsNullOrEmpty(ContentDisposition))
            {
                Response.SetHeader("Content-Disposition", ContentType);
            }

            Response.SetCharsetEncoding(Request.Headers.AcceptCharsetEncoding);

            EncodingManager.FilterResponse(Request, Response);

            using (Stream)
            {
                if (Stream.CanSeek)
                {
                    Stream.Seek(0, SeekOrigin.Begin);
                }

                Stream.CopyTo(Response.Output);
            }
        }
    }
}
