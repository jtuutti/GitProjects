// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using RestFoundation.Resources;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a local HTML file result.
    /// </summary>
    public class HtmlFileResult : IResultAsync
    {
        private const string DefaultHtmlContentType = "text/html";

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the local file path.
        /// </summary>
        public string FilePath { get; set; }

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
        public async Task ExecuteAsync(IServiceContext context, CancellationToken cancellationToken)
        {
            context.Response.Output.Buffer = false;
            context.Response.Output.Clear();
            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);
            context.Response.SetHeader(context.Response.HeaderNames.ContentType, ContentType ?? DefaultHtmlContentType);

            try
            {
                using (var fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                {
                    context.Response.SetHeader(context.Response.HeaderNames.ContentLength, fileStream.Length.ToString(CultureInfo.InvariantCulture));
                    await fileStream.CopyToAsync(context.Response.Output.Stream, Convert.ToInt32(fileStream.Length), cancellationToken);
                }
            }
            catch (ArgumentException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, Global.InvalidFilePathOrUrl);
            }
            catch (FileNotFoundException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, Global.InvalidFilePathOrUrl);
            }
            catch (DirectoryNotFoundException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, Global.InvalidFilePathOrUrl);
            }
            catch (PathTooLongException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, Global.InvalidFilePathOrUrl);
            }
            catch (IOException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, Global.InaccessibleFile);
            }
            catch (SecurityException)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden, Global.InaccessibleFile);
            }
            catch (UnauthorizedAccessException)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden, Global.InaccessibleFile);
            }
        }
    }
}
