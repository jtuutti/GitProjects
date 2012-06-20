using System;
using System.Net;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    public class ContentResult : IResult
    {
        public ContentResult()
        {
            ClearResponse = true;
        }

        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }
        public string Content { get; set; }
        public string ContentType { get; set; }
        public bool ClearResponse { get; set; }

        public void Execute()
        {
            if (Response == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No HTTP context found");
            }

            if (Content == null)
            {
                return;
            }

            if (ClearResponse)
            {
                Response.Clear();
            }

            if (!String.IsNullOrEmpty(ContentType))
            {
                Response.SetHeader("Content-Type", ContentType);
            }

            Response.SetCharsetEncoding(Request.Headers.AcceptCharsetEncoding);
            
            EncodingManager.FilterResponse(Request, Response);

            Response.Write(Content);
        }
    }
}
