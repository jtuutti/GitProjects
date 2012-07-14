using System;
using Newtonsoft.Json;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a JSON result.
    /// </summary>
    public class JsonResult : IResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResult"/> class.
        /// </summary>
        public JsonResult()
        {
            ContentType = "application/json";
        }

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
            context.Response.SetHeader(context.Response.Headers.ContentType, ContentType);
            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);

            OutputCompressionManager.FilterResponse(context);

            var serializer = new JsonSerializer();
            serializer.Serialize(context.Response.Output.Writer, Content);
        }
    }
}
