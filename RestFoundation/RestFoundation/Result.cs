using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using RestFoundation.Results;
using RestFoundation.Runtime;

namespace RestFoundation
{
    public static class Result
    {
        public static StatusResult Ok
        {
            get
            {
                var result = ObjectActivator.Create<StatusResult>();
                result.StatusCode = HttpStatusCode.OK;
                result.StatusDescription = "OK";
                return result;
            }
        }

        public static StatusResult NoContent
        {
            get
            {
                var result = ObjectActivator.Create<StatusResult>();
                result.StatusCode = HttpStatusCode.NoContent;
                result.StatusDescription = String.Empty;
                return result;
            }
        }

        public static StatusResult NotFound
        {
            get
            {
                var result = ObjectActivator.Create<StatusResult>();
                result.StatusCode = HttpStatusCode.NotFound;
                result.StatusDescription = "Not Found";
                return result;
            }
        }

        public static ContentResult Content(string content)
        {
            return Content(content, null, true);
        }

        public static ContentResult Content(string content, bool clearResponse)
        {
            return Content(content, null, clearResponse);
        }

        public static ContentResult Content(string content, string contentType, bool clearResponse)
        {
            var result = ObjectActivator.Create<ContentResult>();
            result.Content = content;
            result.ContentType = contentType;
            result.ClearResponse = clearResponse;

            return result;
        }

        public static BinaryResult Content(byte[] data)
        {
            return Content(data, null, null, true);
        }

        public static BinaryResult Content(byte[] data, bool clearResponse)
        {
            return Content(data, null, null, clearResponse);
        }

        public static BinaryResult Content(byte[] data, string contentType, bool clearResponse)
        {
            return Content(data, contentType, null, clearResponse);
        }

        public static BinaryResult Content(byte[] data, string contentType, string contentDisposition, bool clearResponse)
        {
            var result = ObjectActivator.Create<BinaryResult>();
            result.Content = data;
            result.ContentType = contentType;
            result.ContentDisposition = contentDisposition;
            result.ClearResponse = clearResponse;

            return result;
        }

        public static StreamResult Content(Stream stream)
        {
            return Content(stream, null, null, true);
        }

        public static StreamResult Content(Stream stream, bool clearResponse)
        {
            return Content(stream, null, null, clearResponse);
        }

        public static StreamResult Content(Stream stream, string contentType, bool clearResponse)
        {
            return Content(stream, contentType, null, clearResponse);
        }

        public static StreamResult Content(Stream stream, string contentType, string contentDisposition, bool clearResponse)
        {
            var result = ObjectActivator.Create<StreamResult>();
            result.Stream = stream;
            result.ContentType = contentType;
            result.ContentDisposition = contentDisposition;
            result.ClearResponse = clearResponse;

            return result;
        }

        public static FilePathResult FilePath(string filePath)
        {
            return FilePath(filePath, null, null, true, true);
        }

        public static FilePathResult FilePath(string filePath, bool clearResponse)
        {
            return FilePath(filePath, null, null, clearResponse, true);
        }

        public static FilePathResult FilePath(string filePath, string contentType, bool clearResponse)
        {
            return FilePath(filePath, contentType, null, clearResponse, true);
        }

        public static FilePathResult FilePath(string filePath, string contentType, bool clearResponse, bool cacheOutput)
        {
            return FilePath(filePath, contentType, null, clearResponse, cacheOutput);
        }

        public static FilePathResult FilePath(string filePath, string contentType, string contentDisposition, bool clearResponse, bool cacheOutput)
        {
            var result = ObjectActivator.Create<FilePathResult>();
            result.FilePath = filePath;
            result.ContentType = contentType;
            result.ContentDisposition = contentDisposition;
            result.ClearResponse = clearResponse;
            result.CacheOutput = cacheOutput;

            return result;
        }

        public static FileUrlResult FileUrl(string fileUrl)
        {
            return FileUrl(fileUrl, null, null, true, true);
        }

        public static FileUrlResult FileUrl(string fileUrl, bool clearResponse)
        {
            return FileUrl(fileUrl, null, null, clearResponse, true);
        }

        public static FileUrlResult FileUrl(string fileUrl, string contentType, bool clearResponse)
        {
            return FileUrl(fileUrl, contentType, null, clearResponse, true);
        }

        public static FileUrlResult FileUrl(string fileUrl, string contentType, bool clearResponse, bool cacheOutput)
        {
            return FileUrl(fileUrl, contentType, null, clearResponse, cacheOutput);
        }

        public static FileUrlResult FileUrl(string fileUrl, string contentType, string contentDisposition, bool clearResponse, bool cacheOutput)
        {
            var result = ObjectActivator.Create<FileUrlResult>();
            result.FileUrl = fileUrl;
            result.ContentType = contentType;
            result.ContentDisposition = contentDisposition;
            result.ClearResponse = clearResponse;
            result.CacheOutput = cacheOutput;

            return result;
        }

        public static RedirectResult Redirect(string redirectUrl)
        {
            return Redirect(redirectUrl, false);
        }

        public static RedirectResult Redirect(string redirectUrl, bool isPermanent)
        {
            var result = ObjectActivator.Create<RedirectResult>();
            result.RedirectUrl = redirectUrl;
            result.IsPermanent = isPermanent;

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
            var result = ObjectActivator.Create<StatusResult>();
            result.StatusCode = code;
            result.StatusDescription = description;

            if (additionalHeaders != null)
            {
                foreach (var header in additionalHeaders)
                {
                    result.AdditionalHeaders[header.Key] = header.Value;
                }
            }

            return result;
        }

        public static JsonResult Json(object obj)
        {
            var result = ObjectActivator.Create<JsonResult>();
            result.Content = obj;

            return result;
        }

        public static XmlResult Xml(object obj)
        {
            return Xml(obj, null);
        }

        public static XmlResult Xml(object obj, Type[] extraTypes)
        {
            var result = ObjectActivator.Create<XmlResult>();
            result.Content = obj;
            result.ExtraTypes = extraTypes;

            return result;
        }
    }
}
