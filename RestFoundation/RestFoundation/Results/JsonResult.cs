// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections;
using System.Collections.Generic;
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
        /// Gets or sets the object to serialize to JSON.
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        /// Gets or sets the content type. The "application/json" content type is used by default.
        /// </summary>
        public string ContentType { get; set; }

        internal Type ReturnedType { get; set; }

        /// <summary>
        /// Executes the result against the provided service context.
        /// </summary>
        /// <param name="context">The service context.</param>
        public virtual void Execute(IServiceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Response.Output.Clear();
            context.Response.SetHeader(context.Response.Headers.ContentType, ContentType ?? "application/json");
            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);

            var serializer = new JsonSerializer();

            if (ReturnedType != null && ReturnedType.IsGenericType && ReturnedType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                OutputChunkedSequence(context, serializer);
                return;
            }

            OutputCompressionManager.FilterResponse(context);

            serializer.Serialize(context.Response.Output.Writer, Content);
        }

        private void OutputChunkedSequence(IServiceContext context, JsonSerializer serializer)
        {
            var enumerableContent = (IEnumerable) Content;

            context.Response.Output.Write("[").Flush();

            bool arrayStart = true;

            foreach (object enumeratedContent in enumerableContent)
            {
                if (!arrayStart)
                {
                    context.Response.Output.Write(",");
                }
                else
                {
                    arrayStart = false;
                }

                serializer.Serialize(context.Response.Output.Writer, enumeratedContent);
                context.Response.Output.Flush();
            }

            context.Response.Output.Write("]");
        }
    }
}
