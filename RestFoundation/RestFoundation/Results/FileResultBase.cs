// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a base file result.
    /// This class cannot be instantiated.
    /// </summary>
    public abstract class FileResultBase : IResultAsync
    {
        private const string DefaultBinaryContentType = "application/octet-stream";

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the Content-Disposition HTTP response header value.
        /// </summary>
        public string ContentDisposition { get; set; }

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
            throw new NotSupportedException(RestResources.UnsupportedSyncExecutionForAsyncResult);
        }

        /// <summary>
        /// Executes the result against the provided service context asynchronously.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <returns>A task executing the result.</returns>
        public virtual Task ExecuteAsync(IServiceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            FileInfo file = GetFile(context);

            if (file == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, RestResources.InvalidFilePathOrUrl);
            }

            context.Response.Output.Buffer = false;
            context.Response.Output.Clear();
            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);
            context.Response.SetHeader(context.Response.HeaderNames.AcceptRanges, "bytes");
            context.Response.SetHeader(context.Response.HeaderNames.ContentType, ContentType ?? DefaultBinaryContentType);
            context.Response.SetHeader(context.Response.HeaderNames.ETag, GenerateETag(file));

            if (!String.IsNullOrEmpty(ContentDisposition))
            {
                context.Response.SetHeader(context.Response.HeaderNames.ContentDisposition, ContentDisposition);
            }

            return TransmitFile(context, file);
        }

        /// <summary>
        /// Gets the <see cref="FileInfo"/> instance using the service context.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <returns>The file info instance.</returns>
        protected abstract FileInfo GetFile(IServiceContext context);

        private static async Task TransmitFile(IServiceContext context, FileInfo file)
        {
            var buffer = new byte[4096];

            using (var stream = file.OpenRead())
            {
                CreateRangeOutput(context, stream);

                while (context.Response.IsClientConnected && await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false) > 0)
                {
                    await context.Response.Output.Stream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                    await context.Response.Output.Stream.FlushAsync().ConfigureAwait(false);
                }
            }
        }

        private static void CreateRangeOutput(IServiceContext context, FileStream stream)
        {
            string rangeValue = context.Request.Headers.TryGet("Range");

            if (String.IsNullOrEmpty(rangeValue) || rangeValue.IndexOf("bytes=", StringComparison.OrdinalIgnoreCase) < 0)
            {
                context.Response.SetHeader(context.Response.HeaderNames.ContentLength, stream.Length.ToString(CultureInfo.InvariantCulture));
                return;
            }

            long start;
            string[] ranges = rangeValue.Substring(rangeValue.IndexOf("bytes=", StringComparison.OrdinalIgnoreCase) + 6).Split('-');

            if (!Int64.TryParse(ranges[0], out start))
            {
                return;
            }

            long end;

            if (ranges.Length < 2 || !Int64.TryParse(ranges[1], out end))
            {
                end = stream.Length - 1;
            }

            if (start < 0 || end >= stream.Length || start > end)
            {
                context.Response.SetHeader(context.Response.HeaderNames.ContentLength, stream.Length.ToString(CultureInfo.InvariantCulture));
                context.Response.SetHeader(context.Response.HeaderNames.ContentRange, String.Format(CultureInfo.InvariantCulture, "bytes */{0}", stream.Length));

                throw new HttpResponseException(HttpStatusCode.RequestedRangeNotSatisfiable, RestResources.UnsatisfiableRequestedRange);
            }

            if (start > 0)
            {
                stream.Seek(start, SeekOrigin.Begin);
            }

            context.Response.SetHeader(context.Response.HeaderNames.ContentLength, (end - start + 1).ToString(CultureInfo.InvariantCulture));
            context.Response.SetHeader(context.Response.HeaderNames.ContentRange, String.Format(CultureInfo.InvariantCulture, "bytes {0}-{1}/{2}", start, end, stream.Length));
            context.Response.SetStatus(HttpStatusCode.PartialContent, RestResources.PartialContent);
        }

        private static string GenerateETag(FileInfo file)
        {
            string fileData = String.Format(CultureInfo.CurrentCulture, "{0}:{1}", file.FullName, file.LastWriteTimeUtc.Ticks);

            using (var hasher = new MD5CryptoServiceProvider())
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileData)))
            {
                byte[] hash = hasher.ComputeHash(stream);
                return String.Concat("\"", Convert.ToBase64String(hash).TrimEnd('='), "\"");
            }
        }
    }
}
