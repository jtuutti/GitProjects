using System;
using System.Runtime.Serialization.Json;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    public class DataContractJsonResult : IResult
    {
        public DataContractJsonResult()
        {
            ContentType = "application/json";
        }

        public object Content { get; set; }
        public string ContentType { get; set; }

        public virtual void Execute(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (Content == null)
            {
                return;
            }

            context.Response.Output.Clear();
            context.Response.SetHeader(context.Response.Headers.ContentType, ContentType);
            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);

            OutputCompressionManager.FilterResponse(context);

            var serializer = new DataContractJsonSerializer(Content.GetType());
            serializer.WriteObject(context.Response.Output.Stream, Content);
        }
    }
}
