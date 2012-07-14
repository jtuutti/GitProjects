using System;
using RestFoundation.Context;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a content result.
    /// </summary>
    public class ContentResult : IResult
    {
        /// <summary>
        /// Initializes the new instance of the <see cref="ContentResult"/> class.
        /// </summary>
        public ContentResult()
        {
            ClearOutput = true;
        }

        /// <summary>
        /// Gets or sets the string content.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the response output should be cleared.
        /// </summary>
        public bool ClearOutput { get; set; }

        /// <summary>
        /// Executes the result against the provided service context.
        /// </summary>
        /// <param name="context">The service context.</param>
        public virtual void Execute(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (Content == null)
            {
                return;
            }

            if (ClearOutput)
            {
                context.Response.Output.Clear();
            }

            SetContentType(context);
            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);
            
            OutputCompressionManager.FilterResponse(context);

            context.Response.Output.Write(Content);
        }

        private void SetContentType(IServiceContext context)
        {
            if (!String.IsNullOrEmpty(ContentType))
            {
                context.Response.SetHeader(context.Response.Headers.ContentType, ContentType);
            }
            else
            {
                string acceptType = context.Request.GetPreferredAcceptType();

                if (!String.IsNullOrEmpty(acceptType))
                {
                    context.Response.SetHeader(context.Response.Headers.ContentType, acceptType);
                }
            }
        }
    }
}
