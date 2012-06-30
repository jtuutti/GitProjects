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
        
        public IDictionary<string, string> AdditionalHeaders { get; protected set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }

        public void Execute(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            foreach (var header in AdditionalHeaders)
            {
                context.Response.SetHeader(header.Key, header.Value);
            }

            context.Response.SetStatus(StatusCode, StatusDescription ?? String.Empty);
        }
    }
}
