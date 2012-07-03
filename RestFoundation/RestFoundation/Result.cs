using System;
using System.Collections.Generic;
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
                                 StatusDescription = String.Empty
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
            return Content(content, null, true);
        }

        public static ContentResult Content(string content, bool clearOutput)
        {
            return Content(content, null, clearOutput);
        }

        public static ContentResult Content(string content, string contentType, bool clearOutput)
        {
            var result = new ContentResult
                         {
                             Content = content,
                             ContentType = contentType,
                             ClearOutput = clearOutput
                         };

            return result;
        }

        public static ContentResult ContentFormat(string format, params object[] args)
        {
            if (format == null) throw new ArgumentNullException("format");

            return Content(String.Format(format, args), null, true);
        }

        public static ContentResult ContentFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (format == null) throw new ArgumentNullException("format");

            return Content(String.Format(provider, format, args), null, true);
        }

        public static BinaryResult Content(byte[] data)
        {
            return Content(data, null, null, true);
        }

        public static BinaryResult Content(byte[] data, bool clearOutput)
        {
            return Content(data, null, null, clearOutput);
        }

        public static BinaryResult Content(byte[] data, string contentType, bool clearOutput)
        {
            return Content(data, contentType, null, clearOutput);
        }

        public static BinaryResult Content(byte[] data, string contentType, string contentDisposition, bool clearOutput)
        {
            var result = new BinaryResult
                         {
                             Content = data,
                             ContentType = contentType,
                             ContentDisposition = contentDisposition,
                             ClearOutput = clearOutput
                         };

            return result;
        }

        public static StreamResult Content(Stream stream)
        {
            return Content(stream, null, null, true);
        }

        public static StreamResult Content(Stream stream, bool clearOutput)
        {
            return Content(stream, null, null, clearOutput);
        }

        public static StreamResult Content(Stream stream, string contentType, bool clearOutput)
        {
            return Content(stream, contentType, null, clearOutput);
        }

        public static StreamResult Content(Stream stream, string contentType, string contentDisposition, bool clearOutput)
        {
            var result = new StreamResult
                         {
                             Stream = stream,
                             ContentType = contentType,
                             ContentDisposition = contentDisposition,
                             ClearOutput = clearOutput
                         };

            return result;
        }

        public static FilePathResult FilePath(string filePath)
        {
            return FilePath(filePath, null, null, true, TimeSpan.Zero);
        }

        public static FilePathResult FilePath(string filePath, bool clearOutput)
        {
            return FilePath(filePath, null, null, clearOutput, TimeSpan.Zero);
        }

        public static FilePathResult FilePath(string filePath, string contentType, bool clearOutput)
        {
            return FilePath(filePath, contentType, null, clearOutput, TimeSpan.Zero);
        }

        public static FilePathResult FilePath(string filePath, string contentType, bool clearOutput, TimeSpan maxAge)
        {
            return FilePath(filePath, contentType, null, clearOutput, maxAge);
        }

        public static FilePathResult FilePath(string filePath, string contentType, string contentDisposition, bool clearOutput, TimeSpan maxAge)
        {
            var result = new FilePathResult
                         {
                             FilePath = filePath,
                             ContentType = contentType,
                             ContentDisposition = contentDisposition,
                             ClearOutput = clearOutput,
                             CacheForTimeSpan = maxAge
                         };

            return result;
        }

        public static FileUrlResult FileUrl(string fileUrl)
        {
            return FileUrl(fileUrl, null, null, true, TimeSpan.Zero);
        }

        public static FileUrlResult FileUrl(string fileUrl, bool clearOutput)
        {
            return FileUrl(fileUrl, null, null, clearOutput, TimeSpan.Zero);
        }

        public static FileUrlResult FileUrl(string fileUrl, string contentType, bool clearOutput)
        {
            return FileUrl(fileUrl, contentType, null, clearOutput, TimeSpan.Zero);
        }

        public static FileUrlResult FileUrl(string fileUrl, string contentType, bool clearOutput, bool cacheOutput, TimeSpan maxAge)
        {
            return FileUrl(fileUrl, contentType, null, clearOutput, maxAge);
        }

        public static FileUrlResult FileUrl(string fileUrl, string contentType, string contentDisposition, bool clearOutput, TimeSpan maxAge)
        {
            var result = new FileUrlResult
                         {
                             FileUrl = fileUrl,
                             ContentType = contentType,
                             ContentDisposition = contentDisposition,
                             ClearOutput = clearOutput,
                             CacheForTimeSpan = maxAge
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
