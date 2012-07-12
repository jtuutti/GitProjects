using System;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    public class JsonPResult : IResult
    {
        private static readonly Regex methodNamePattern = new Regex("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public object Content { get; set; }
        public string Callback { get; set; }

        public virtual void Execute(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (String.IsNullOrEmpty(Callback))
            {
                Callback = "jsonpCallback";
            }
            else if (!methodNamePattern.IsMatch(Callback))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid JSONP callback method provided");
            }

            context.Response.Output.Clear();
            context.Response.SetHeader(context.Response.Headers.ContentType, "application/javascript");
            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);

            OutputCompressionManager.FilterResponse(context);

            context.Response.Output.Write(Callback)
                                   .Write("(")
                                   .Write(Content != null ? JsonConvert.SerializeObject(Content) : "null")
                                   .Write(");");
        }
    }
}
