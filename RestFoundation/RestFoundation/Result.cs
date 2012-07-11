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
        public static StatusResult Accepted
        {
            get
            {
                var result = new StatusResult
                             {
                                 StatusCode = HttpStatusCode.Accepted,
                                 StatusDescription = "Accepted"
                             };

                return result;
            }
        }

        public static StatusResult BadRequest
        {
            get
            {
                var result = new StatusResult
                             {
                                 StatusCode = HttpStatusCode.BadRequest,
                                 StatusDescription = "Bad Request"
                             };

                return result;
            }
        }

        public static StatusResult Conflict
        {
            get
            {
                var result = new StatusResult
                             {
                                 StatusCode = HttpStatusCode.Conflict,
                                 StatusDescription = "Conflict"
                             };

                return result;
            }
        }

        public static StatusResult Forbidden
        {
            get
            {
                var result = new StatusResult
                             {
                                 StatusCode = HttpStatusCode.Forbidden,
                                 StatusDescription = "Forbidden"
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

        public static StatusResult NotImplemented
        {
            get
            {
                var result = new StatusResult
                             {
                                 StatusCode = HttpStatusCode.NotImplemented,
                                 StatusDescription = "Not Implemented"
                             };

                return result;
            }
        }

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

        public static StatusResult ServiceUnavailable
        {
            get
            {
                var result = new StatusResult
                             {
                                 StatusCode = HttpStatusCode.ServiceUnavailable,
                                 StatusDescription = "Service Unavailable"
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

        public static BinaryResult Binary(byte[] data)
        {
            return Binary(data, true, null, null);
        }

        public static BinaryResult Binary(byte[] data, bool clearOutput)
        {
            return Binary(data, clearOutput, null, null);
        }

        public static BinaryResult Binary(byte[] data, bool clearOutput, string contentType)
        {
            return Binary(data, clearOutput, contentType, null);
        }

        public static BinaryResult Binary(byte[] data, bool clearOutput, string contentType, string contentDisposition)
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

        public static StreamResult Stream(Stream stream)
        {
            return Stream(stream, true, null, null);
        }

        public static StreamResult Stream(Stream stream, bool clearOutput)
        {
            return Stream(stream, clearOutput, null, null);
        }

        public static StreamResult Stream(Stream stream, bool clearOutput, string contentType)
        {
            return Stream(stream, clearOutput, contentType, null);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
                         Justification = "The result will be disposed by the REST handler")]
        public static StreamResult Stream(Stream stream, bool clearOutput, string contentType, string contentDisposition)
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

        public static FilePathResult LocalFile(string filePath)
        {
            return LocalFile(filePath, true, null, TimeSpan.Zero, null);
        }

        public static FilePathResult LocalFile(string filePath, bool clearOutput)
        {
            return LocalFile(filePath, clearOutput, null, TimeSpan.Zero, null);
        }

        public static FilePathResult LocalFile(string filePath, bool clearOutput, string contentType)
        {
            return LocalFile(filePath, clearOutput, contentType, TimeSpan.Zero, null);
        }

        public static FilePathResult LocalFile(string filePath, bool clearOutput, string contentType, TimeSpan maxAge)
        {
            return LocalFile(filePath, clearOutput, contentType, maxAge, null);
        }

        public static FilePathResult LocalFile(string filePath, bool clearOutput, string contentType, TimeSpan maxAge, string contentDisposition)
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

        public static FileUrlResult RemoteFile(string fileUrl)
        {
            return RemoteFile(fileUrl, true, null, TimeSpan.Zero, null);
        }

        public static FileUrlResult RemoteFile(string fileUrl, bool clearOutput)
        {
            return RemoteFile(fileUrl, clearOutput, null, TimeSpan.Zero, null);
        }

        public static FileUrlResult RemoteFile(string fileUrl, bool clearOutput, string contentType)
        {
            return RemoteFile(fileUrl, clearOutput, contentType, TimeSpan.Zero, null);
        }

        public static FileUrlResult RemoteFile(string fileUrl, bool clearOutput, string contentType, TimeSpan maxAge)
        {
            return RemoteFile(fileUrl, clearOutput, contentType, maxAge, null);
        }

        public static FileUrlResult RemoteFile(string fileUrl, bool clearOutput, string contentType, TimeSpan maxAge, string contentDisposition)
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

        public static IResult Resource(object obj)
        {
            return Resource(obj, HttpStatusCode.OK, "OK", null);
        }

        public static IResult Resource(object obj, HttpStatusCode statusCode)
        {
            return Resource(obj, statusCode, null, null);
        }

        public static IResult Resource(object obj, HttpStatusCode statusCode, string statusDescription)
        {
            return Resource(obj, statusCode, statusDescription, null);
        }

        public static IResult Resource(object obj, HttpStatusCode statusCode, string statusDescription, IDictionary<string, string> additionalHeaders)
        {
            var context = Rest.Active.CreateObject<IServiceContext>();
            var resultFactory = Rest.Active.CreateObject<IResultFactory>();

            IResult statusResult = ResponseStatus(statusCode, statusDescription, additionalHeaders);
            IResult resourceResult = resultFactory.Create(context, obj);

            statusResult.Execute(context);
            return resourceResult;
        }

        public static StatusResult ResponseStatus(HttpStatusCode code)
        {
            return ResponseStatus(code, String.Empty, null);
        }

        public static StatusResult ResponseStatus(HttpStatusCode code, string description)
        {
            return ResponseStatus(code, description, null);
        }

        public static StatusResult ResponseStatus(HttpStatusCode code, string description, IDictionary<string, string> additionalHeaders)
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

        public static JsonResult Json(object obj)
        {
            if (Rest.Active.DataContractSerializers)
            {
                return new DataContractJsonResult
                {
                    Content = obj
                };
            }

            return new JsonResult
            {
                Content = obj
            };
        }

        public static JsonPResult JsonP(object obj)
        {
            return JsonP(obj, null);
        }

        public static JsonPResult JsonP(object obj, string callback)
        {
            var result = new JsonPResult
            {
                Content = obj,
                Callback = callback
            };

            return result;
        }

        public static XmlResult Xml(object obj)
        {
            if (Rest.Active.DataContractSerializers)
            {
                return new DataContractXmlResult
                {
                    Content = obj
                };
            }

            return new XmlResult
            {
                Content = obj
            };
        }
    }
}
