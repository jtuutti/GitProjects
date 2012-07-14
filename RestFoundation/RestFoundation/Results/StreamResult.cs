using System;
using System.IO;
using System.Net;
using RestFoundation.Context;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a stream data result.
    /// </summary>
    public class StreamResult : IResult, IDisposable
    {
        private bool m_isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamResult"/> class.
        /// </summary>
        public StreamResult()
        {
            ClearOutput = true;
        }

        /// <summary>
        /// Gets or sets the input data stream.
        /// </summary>
        public Stream Stream { get; set; }

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

            if (Stream == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No valid input stream provided");
            }

            if (ClearOutput)
            {
                context.Response.Output.Clear();
            }

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

            if (!String.IsNullOrEmpty(ContentDisposition))
            {
                context.Response.SetHeader(context.Response.Headers.ContentDisposition, ContentType);
            }

            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);

            OutputCompressionManager.FilterResponse(context);

            using (Stream)
            {
                if (Stream.CanSeek)
                {
                    Stream.Seek(0, SeekOrigin.Begin);
                }

                Stream.CopyTo(context.Response.Output.Stream);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);  
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="StreamResult"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// true to release both managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (m_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                if (Stream != null)
                {
                    try
                    {
                        Stream.Flush();
                    }
                    catch (Exception)
                    {
                    }

                    Stream.Dispose();
                }
            }

            m_isDisposed = true;   
        }
    }
}
