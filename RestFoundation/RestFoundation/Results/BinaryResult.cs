using System;
using System.Net;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    public class BinaryResult : IResult
    {
        public BinaryResult()
        {
            ClearResponse = true;
        }

        public IServiceContext Context { get; set; }
        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
        public string ContentDisposition { get; set; }
        public bool ClearResponse { get; set; }

        public void Execute()
        {
            if (Response == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No HTTP context found");
            }

            if (Content == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No valid binary content provided");
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

            OutputCompressionManager.FilterResponse(Request, Response);

            if (Content.Length > 0)
            {
                Response.Output.Write(Content, 0, Content.Length);
            }
        }
    }
}
