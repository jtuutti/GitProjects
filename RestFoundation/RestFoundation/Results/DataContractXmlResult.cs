using System;
using System.Runtime.Serialization;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a data contract serialized XML result.
    /// </summary>
    public class DataContractXmlResult : IResult
    {
        /// <summary>
        /// Gets or sets the object to serialize to JSON.
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        /// Gets or sets the content type. The "application/json" content type is used by default.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Executes the result against the provided service context.
        /// </summary>
        /// <param name="context">The service context.</param>
        public virtual void Execute(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            context.Response.Output.Clear();
            context.Response.SetHeader(context.Response.Headers.ContentType, ContentType ?? "application/xml");
            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);

            OutputCompressionManager.FilterResponse(context);

            var serializer = new DataContractSerializer(Content != null ? Content.GetType() : typeof(object));

// ReSharper disable AssignNullToNotNullAttribute - wrong Resharper logic
            serializer.WriteObject(context.Response.Output.Stream, Content);
// ReSharper restore AssignNullToNotNullAttribute
        }
    }
}
