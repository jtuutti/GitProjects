// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a content result.
    /// </summary>
    public class ContentResult : IResult
    {
        private readonly IContentNegotiator m_contentNegotiator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentResult"/> class.
        /// </summary>
        public ContentResult() : this(Rest.Configuration.ServiceLocator.GetService<IContentNegotiator>()) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentResult"/> class with the provided content negotiator.
        /// </summary>
        /// <param name="contentNegotiator">The content negotiator.</param>
        public ContentResult(IContentNegotiator contentNegotiator)
        {
            if (contentNegotiator == null)
            {
                throw new ArgumentNullException("contentNegotiator");
            }

            m_contentNegotiator = contentNegotiator;
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
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

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

            LogResponse();
        }

        private void SetContentType(IServiceContext context)
        {
            if (!String.IsNullOrEmpty(ContentType))
            {
                context.Response.SetHeader(context.Response.HeaderNames.ContentType, ContentType);
            }
            else
            {
                string acceptType = m_contentNegotiator.GetPreferredMediaType(context.Request);

                if (!String.IsNullOrEmpty(acceptType))
                {
                    context.Response.SetHeader(context.Response.HeaderNames.ContentType, acceptType);
                }
            }
        }

        private void LogResponse()
        {
            if (Content != null && LogUtility.CanLog)
            {
                LogUtility.LogResponseBody(Content, ContentType);
            }
        }
    }
}
