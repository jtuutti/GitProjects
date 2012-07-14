﻿using System;
using System.Diagnostics.CodeAnalysis;
using RestFoundation.Context;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a binary data result.
    /// </summary>
    public class BinaryResult : IResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryResult"/> class.
        /// </summary>
        public BinaryResult()
        {
            ClearOutput = true;
        }

        /// <summary>
        /// Gets or sets the binary content.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
                         Justification = "byte[] is a natural representation of in-memory binary data")]
        public byte[] Content { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the Content-Disposition HTTP response header value.
        /// </summary>
        public string ContentDisposition { get; set; }

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

            if (!String.IsNullOrEmpty(ContentDisposition))
            {
                context.Response.SetHeader(context.Response.Headers.ContentDisposition, ContentType);
            }

            OutputCompressionManager.FilterResponse(context);

            if (Content.Length > 0)
            {
                context.Response.Output.Stream.Write(Content, 0, Content.Length);
            }
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
