using System.Net;
using Newtonsoft.Json;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    public class JsonResult : IResult
    {
        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }
        public object Content { get; set; }

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

            Response.Clear();
            Response.SetHeader("Content-Type", "application/json");
            Response.SetCharsetEncoding(Request.Headers.AcceptCharsetEncoding);

            EncodingManager.FilterResponse(Request, Response);

            var serializer = new JsonSerializer();
            serializer.Serialize(Response.OutputWriter, Content);
        }
    }
}
