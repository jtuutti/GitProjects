using System.Net;
using RestFoundation.DataFormatters;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    internal sealed class FormatterResult : IResult
    {
        public IServiceContext Context { get; set; }
        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }
        public object ReturnedObject { get; set; }

        public void Execute()
        {
            IDataFormatter formatter = DataFormatterRegistry.GetFormatter(Request.GetPreferredAcceptType());

            if (formatter == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable, "No supported content type was provided in the Accept or the Content-Type header");
            }

            formatter.FormatResponse(Request, Response, ReturnedObject);
        }
    }
}
