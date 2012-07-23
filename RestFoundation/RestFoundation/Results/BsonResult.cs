using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a BSON result.
    /// </summary>
    public class BsonResult : IResult
    {
        /// <summary>
        /// Gets or sets the object to serialize to BSON.
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        /// Gets or sets the content type.
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
            context.Response.SetHeader(context.Response.Headers.ContentType, ContentType ?? "application/bson");
            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);

            OutputCompressionManager.FilterResponse(context);

            var serializer = new JsonSerializer();
            var bsonWriter = new BsonWriter(context.Response.Output.Stream);
            serializer.Serialize(bsonWriter, Content);
        }
    }
}
