using System;
using System.IO;
using System.Net;
using RestFoundation.Context;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a base file result.
    /// </summary>
    public abstract class FileResultBase : IResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileResultBase"/> class.
        /// </summary>
        protected FileResultBase()
        {
            ClearOutput = true;
        }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the Content-Disposition HTTP response header value.
        /// </summary>
        public string ContentDisposition { get; set; }

        /// <summary>
        /// Gets or sets the amount of time that a cache entry is to remain in the output cache.
        /// <see cref="TimeSpan.Zero"/> means no caching should occur.
        /// </summary>
        public TimeSpan CacheDuration { get; set; }

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

            FileInfo file = GetFile(context);

            if (file == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No valid file path/URL provided");
            }

            if (ClearOutput)
            {
                context.Response.Output.Clear();
            }

            SetContentType(context);

            if (!String.IsNullOrEmpty(ContentDisposition))
            {
                context.Response.SetHeader(context.Response.Headers.ContentDisposition, ContentType);
            }

            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);

            OutputCompressionManager.FilterResponse(context);

            context.Response.SetFileDependencies(file.FullName, CacheDuration);
            TransmitFile(context, file.FullName);
        }

        /// <summary>
        /// Gets the <see cref="FileInfo"/> instance using the service context.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <returns>The file info instance.</returns>
        protected abstract FileInfo GetFile(IServiceContext context);

        private static void TransmitFile(IServiceContext context, string filePath)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (filePath == null) throw new ArgumentNullException("filePath");

            context.GetHttpContext().Response.TransmitFile(filePath);
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
