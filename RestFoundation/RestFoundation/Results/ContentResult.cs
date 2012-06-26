using System;
using System.Net;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    public class ContentResult : IResult
    {
        public ContentResult()
        {
            ClearOutput = true;
        }

        public IServiceContext Context { get; set; }
        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }
        public string Content { get; set; }
        public string ContentType { get; set; }
        public bool ClearOutput { get; set; }

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

            if (ClearOutput)
            {
                Response.Output.Clear();
            }

            if (!String.IsNullOrEmpty(ContentType))
            {
                Response.SetHeader("Content-Type", ContentType);
            }
            else
            {
                string acceptType = Request.GetPreferredAcceptType();

                if (!String.IsNullOrEmpty(acceptType))
                {
                    Response.SetHeader("Content-Type", acceptType);
                }
            }

            Response.SetCharsetEncoding(Request.Headers.AcceptCharsetEncoding);
            
            OutputCompressionManager.FilterResponse(Request, Response);

            Response.Output.Write(Content);
        }
    }
}
