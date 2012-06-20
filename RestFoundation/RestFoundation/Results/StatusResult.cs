using System;
using System.Collections.Generic;
using System.Net;

namespace RestFoundation.Results
{
    public class StatusResult : IResult
    {
        public StatusResult()
        {
            AdditionalHeaders = new Dictionary<string, string>();
        }
        
        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }
        public IDictionary<string, string> AdditionalHeaders { get; protected set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }

        public void Execute()
        {
            if (Response == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No HTTP context found");
            }

            foreach (var header in AdditionalHeaders)
            {
                Response.SetHeader(header.Key, header.Value);
            }

            Response.SetStatus(StatusCode, StatusDescription ?? String.Empty);
        }
    }
}
 