// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RestFoundation.Resources;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a stream result.
    /// </summary>
    public class StreamResult : IResultAsync
    {
        private const int DefaultBuffer = 16384;
        private readonly IContentNegotiator m_contentNegotiator;

        private int m_buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamResult"/> class.
        /// </summary>
        public StreamResult() : this(Rest.Configuration.ServiceLocator.GetService<IContentNegotiator>()) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamResult"/> class with the provided content negotiator.
        /// </summary>
        /// <param name="contentNegotiator">The content negotiator.</param>
        public StreamResult(IContentNegotiator contentNegotiator)
        {
            if (contentNegotiator == null)
            {
                throw new ArgumentNullException("contentNegotiator");
            }

            m_contentNegotiator = contentNegotiator;
            m_buffer = DefaultBuffer;

            ClearOutput = true;
        }

        /// <summary>
        /// Gets or sets the stream read buffer size.
        /// </summary>
        public int Buffer
        {
            get
            {
                return m_buffer;
            }
            set
            {
                if (m_buffer <= 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                m_buffer = value;
            }
        }

        /// <summary>
        /// Gets or sets the content stream.
        /// </summary>
        public Stream Stream { get; set; }

        /// <summary>
        /// Gets or sets the Content-Disposition HTTP response header value.
        /// </summary>
        public string ContentDisposition { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the response output should be cleared.
        /// </summary>
        public bool ClearOutput { get; set; }

        /// <summary>
        /// Executes the result against the provided service synchronously.
        /// Asynchronous method should throw a <see cref="NotSupportedException"/> and implement
        /// the <see cref="IResultAsync.ExecuteAsync"/> method instead.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <exception cref="NotSupportedException">
        /// When called from a service method result that implements the <see cref="IResultAsync"/>
        /// interface.
        /// </exception>
        public void Execute(IServiceContext context)
        {
            throw new NotSupportedException(Global.UnsupportedSyncExecutionForAsyncResult);
        }

        /// <summary>
        /// Executes the result against the provided service context asynchronously.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task executing the result.</returns>
        public virtual async Task ExecuteAsync(IServiceContext context, CancellationToken cancellationToken)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (Stream == null)
            {
                return;
            }

            context.Response.Output.Buffer = false;

            if (ClearOutput)
            {
                context.Response.Output.Clear();
            }

            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);
            SetContentType(context);

            if (!String.IsNullOrEmpty(ContentDisposition))
            {
                context.Response.SetHeader(context.Response.HeaderNames.ContentDisposition, ContentDisposition);
            }

            using (Stream)
            {
                if (Stream.CanSeek)
                {
                    Stream.Position = 0;
                    context.Response.SetHeader(context.Response.HeaderNames.ContentLength, Stream.Length.ToString(CultureInfo.InvariantCulture));
                }

                var buffer = new byte[m_buffer < Stream.Length ? m_buffer : Stream.Length];
                int bytesRead;

                while ((bytesRead = await Stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0 && context.Response.IsClientConnected)
                {
                    await context.Response.Output.Stream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                    await context.Response.Output.Stream.FlushAsync(cancellationToken);
                }
            }
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
    }
}
