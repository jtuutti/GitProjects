// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a base file result.
    /// This class cannot be instantiated.
    /// </summary>
    public abstract class FileResultBase : IResult
    {
        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the Content-Disposition HTTP response header value.
        /// </summary>
        public string ContentDisposition { get; set; }

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

            FileInfo file = GetFile(context);

            if (file == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, RestResources.InvalidFilePathOrUrl);
            }

            context.Response.Output.Buffer = false;
            context.Response.Output.Clear();
            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);
            context.Response.SetHeader(context.Response.HeaderNames.AcceptRanges, "bytes");
            context.Response.SetHeader(context.Response.HeaderNames.ContentLength, file.Length.ToString(CultureInfo.InvariantCulture));
            context.Response.SetHeader(context.Response.HeaderNames.ContentType, ContentType ?? "application/octet-stream");
            context.Response.SetHeader(context.Response.HeaderNames.ETag, GenerateETag(file));

            if (!String.IsNullOrEmpty(ContentDisposition))
            {
                context.Response.SetHeader(context.Response.HeaderNames.ContentDisposition, ContentDisposition);
            }

            TransmitFile(context, file);
        }

        /// <summary>
        /// Gets the <see cref="FileInfo"/> instance using the service context.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <returns>The file info instance.</returns>
        protected abstract FileInfo GetFile(IServiceContext context);

        private static void TransmitFile(IServiceContext context, FileInfo file)
        {
            var buffer = new byte[4096];

            using (var stream = file.OpenRead())
            {
                CreateRangeOutput(context, stream);

                while (context.GetHttpContext().Response.IsClientConnected && stream.Read(buffer, 0, buffer.Length) > 0)
                {
                    context.Response.Output.Stream.Write(buffer, 0, buffer.Length);                   
                    context.Response.Output.Flush();
                }
            }
        }

        private static void CreateRangeOutput(IServiceContext context, FileStream stream)
        {
            string rangeValue = context.Request.Headers.TryGet("Range");

            if (String.IsNullOrEmpty(rangeValue) || rangeValue.IndexOf("bytes=", StringComparison.OrdinalIgnoreCase) < 0)
            {
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
                context.Response.SetHeader(context.Response.HeaderNames.ContentRange, String.Format(CultureInfo.InvariantCulture, "bytes */{0}", stream.Length));
                throw new HttpResponseException(HttpStatusCode.RequestedRangeNotSatisfiable, RestResources.UnsatisfiableRequestedRange);
            }

            if (start > 0)
            {
                stream.Seek(start, SeekOrigin.Begin);
            }

            context.Response.SetHeader(context.Response.HeaderNames.ContentLength, (end - start + 1).ToString(CultureInfo.InvariantCulture));
            context.Response.SetHeader(context.Response.HeaderNames.ContentRange, String.Format(CultureInfo.InvariantCulture, "bytes {0}-{1}/{2}", start, end, stream.Length));
            context.Response.SetStatus(HttpStatusCode.PartialContent, "Partial content");
        }

        private static string GenerateETag(FileInfo file)
        {
            string fileData = String.Format(CultureInfo.CurrentCulture, "{0}:{1}", file.FullName, file.LastWriteTimeUtc.Ticks);

            using (var hasher = new MD5CryptoServiceProvider())
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileData)))
            {
                var hash = hasher.ComputeHash(stream);
                return String.Concat("\"", Convert.ToBase64String(hash).TrimEnd('='), "\"");
            }
        }
    }
}
