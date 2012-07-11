using System;
using System.Runtime.Serialization;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    public class DataContractXmlResult : XmlResult
    {
        public override void Execute(IServiceContext context)
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

            var serializer = new DataContractSerializer(Content.GetType());
            serializer.WriteObject(context.Response.Output.Stream, Content);
        }
    }
}
