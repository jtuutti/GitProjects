using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using RestFoundation.Results;

namespace RestFoundation
{
    public static class Result
    {
        public static StatusResult Ok
        {
            get
            {
                var result = new StatusResult
                             {
                                 StatusCode = HttpStatusCode.OK,
                                 StatusDescription = "OK"
                             };

                return result;
            }
        }

        public static StatusResult NoContent
        {
            get
            {
                var result = new StatusResult
                             {
                                 StatusCode = HttpStatusCode.NoContent,
                                 StatusDescription = "No Content"
                             };

                return result;
            }
        }

        public static StatusResult NotFound
        {
            get
            {
                var result = new StatusResult
                             {
                                 StatusCode = HttpStatusCode.NotFound,
                                 StatusDescription = "Not Found"
                             };

                return result;
            }
        }

        public static ContentResult Content(string content)
        {
            return Content(content, true, null);
        }

        public static ContentResult Content(string content, bool clearOutput)
        {
            return Content(content, clearOutput, null);
        }

        public static ContentResult Content(string content, bool clearOutput, string contentType)
        {
            var result = new ContentResult
                         {
                             Content = content,
                             ClearOutput = clearOutput,
                             ContentType = contentType
                         };

            return result;
        }

        public static ContentResult ContentFormat(string format, params object[] args)
        {
            if (format == null) throw new ArgumentNullException("format");

            return Content(String.Format(CultureInfo.InvariantCulture, format, args), true, null);
        }

        public static ContentResult ContentFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (format == null) throw new ArgumentNullException("format");

            return Content(String.Format(provider, format, args), true, null);
        }

        public static BinaryResult Content(byte[] data)
        {
            return Content(data, true, null, null);
        }

        public static BinaryResult Content(byte[] data, bool clearOutput)
        {
            return Content(data, clearOutput, null, null);
        }

        public static BinaryResult Content(byte[] data, bool clearOutput, string contentType)
        {
            return Content(data, clearOutput, contentType, null);
        }

        public static BinaryResult Content(byte[] data, bool clearOutput, string contentType, string contentDisposition)
        {
            var result = new BinaryResult
                         {
                             Content = data,
                             ClearOutput = clearOutput,
                             ContentType = contentType,
                             ContentDisposition = contentDisposition
                         };

            return result;
        }

        public static StreamResult Content(Stream stream)
        {
            return Content(stream, true, null, null);
        }

        public static StreamResult Content(Stream stream, bool clearOutput)
        {
            return Content(stream, clearOutput, null, null);
        }

        public static StreamResult Content(Stream stream, bool clearOutput, string contentType)
        {
            return Content(stream, clearOutput, contentType, null);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
                         Justification = "The result will be disposed by the REST handler")]
        public static StreamResult Content(Stream stream, bool clearOutput, string contentType, string contentDisposition)
        {
            var result = new StreamResult
                         {
                             Stream = stream,
                             ClearOutput = clearOutput,
                             ContentType = contentType,
                             ContentDisposition = contentDisposition
                         };

            return result;
        }

        public static FilePathResult FilePath(string filePath)
        {
            return FilePath(filePath, true, null, TimeSpan.Zero, null);
        }

        public static FilePathResult FilePath(string filePath, bool clearOutput)
        {
            return FilePath(filePath, clearOutput, null, TimeSpan.Zero, null);
        }

        public static FilePathResult FilePath(string filePath, bool clearOutput, string contentType)
        {
            return FilePath(filePath, clearOutput, contentType, TimeSpan.Zero, null);
        }

        public static FilePathResult FilePath(string filePath, bool clearOutput, string contentType, TimeSpan maxAge)
        {
            return FilePath(filePath, clearOutput, contentType, maxAge, null);
        }

        public static FilePathResult FilePath(string filePath, bool clearOutput, string contentType, TimeSpan maxAge, string contentDisposition)
        {
            var result = new FilePathResult
                         {
                             FilePath = filePath,
                             ClearOutput = clearOutput,
                             ContentType = contentType,
                             CacheForTimeSpan = maxAge,
                             ContentDisposition = contentDisposition
                         };

            return result;
        }

        public static FileUrlResult FileUrl(string fileUrl)
        {
            return FileUrl(fileUrl, true, null, TimeSpan.Zero, null);
        }

        public static FileUrlResult FileUrl(string fileUrl, bool clearOutput)
        {
            return FileUrl(fileUrl, clearOutput, null, TimeSpan.Zero, null);
        }

        public static FileUrlResult FileUrl(string fileUrl, bool clearOutput, string contentType)
        {
            return FileUrl(fileUrl, clearOutput, contentType, TimeSpan.Zero, null);
        }

        public static FileUrlResult FileUrl(string fileUrl, bool clearOutput, string contentType, TimeSpan maxAge)
        {
            return FileUrl(fileUrl, clearOutput, contentType, maxAge, null);
        }

        public static FileUrlResult FileUrl(string fileUrl, bool clearOutput, string contentType, TimeSpan maxAge, string contentDisposition)
        {
            var result = new FileUrlResult
                         {
                             FileUrl = fileUrl,
                             ClearOutput = clearOutput,
                             ContentType = contentType,
                             CacheForTimeSpan = maxAge,
                             ContentDisposition = contentDisposition
                         };

            return result;
        }

        public static StatusResult SetStatus(HttpStatusCode code)
        {
            return SetStatus(code, String.Empty, null);
        }

        public static StatusResult SetStatus(HttpStatusCode code, string description)
        {
            return SetStatus(code, description, null);
        }

        public static StatusResult SetStatus(HttpStatusCode code, string description, IDictionary<string, string> additionalHeaders)
        {
            var result = new StatusResult
                         {
                             StatusCode = code,
                             StatusDescription = description
                         };

            if (additionalHeaders != null)
            {
                foreach (var header in additionalHeaders)
                {
                    result.AdditionalHeaders[header.Key] = header.Value;
                }
            }

            return result;
        }

        public static FeedResult Feed(SyndicationFeed feed, FeedResult.SyndicationFormat format)
        {
            return Feed(feed, format, false);
        }

        public static FeedResult Feed(SyndicationFeed feed, FeedResult.SyndicationFormat format, bool xmlStyleDates)
        {
            var result = new FeedResult
                         {
                             Feed = feed,
                             Format = format,
                             XmlStyleDates = xmlStyleDates
                         };

            return result;
        }
    }
}
