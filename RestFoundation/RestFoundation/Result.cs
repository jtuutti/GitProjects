﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using RestFoundation.Results;

namespace RestFoundation
{
    public static class Result
    {
        public static StatusResult Ok
        {
            get
            {
                var result = Rest.Active.CreateObject<StatusResult>();
                result.StatusCode = HttpStatusCode.OK;
                result.StatusDescription = "OK";
                return result;
            }
        }

        public static StatusResult NoContent
        {
            get
            {
                var result = Rest.Active.CreateObject<StatusResult>();
                result.StatusCode = HttpStatusCode.NoContent;
                result.StatusDescription = String.Empty;
                return result;
            }
        }

        public static StatusResult NotFound
        {
            get
            {
                var result = Rest.Active.CreateObject<StatusResult>();
                result.StatusCode = HttpStatusCode.NotFound;
                result.StatusDescription = "Not Found";
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
            var result = Rest.Active.CreateObject<ContentResult>();
            result.Content = content;
            result.ContentType = contentType;
            result.ClearOutput = clearOutput;

            return result;
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
            var result = Rest.Active.CreateObject<BinaryResult>();
            result.Content = data;
            result.ContentType = contentType;
            result.ContentDisposition = contentDisposition;
            result.ClearOutput = clearOutput;

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
            var result = Rest.Active.CreateObject<StreamResult>();
            result.Stream = stream;
            result.ContentType = contentType;
            result.ContentDisposition = contentDisposition;
            result.ClearOutput = clearOutput;

            return result;
        }

        public static FilePathResult FilePath(string filePath)
        {
            return FilePath(filePath, null, null, true, true);
        }

        public static FilePathResult FilePath(string filePath, bool clearOutput)
        {
            return FilePath(filePath, null, null, clearOutput, true);
        }

        public static FilePathResult FilePath(string filePath, string contentType, bool clearOutput)
        {
            return FilePath(filePath, contentType, null, clearOutput, true);
        }

        public static FilePathResult FilePath(string filePath, string contentType, bool clearOutput, bool cacheOutput)
        {
            return FilePath(filePath, contentType, null, clearOutput, cacheOutput);
        }

        public static FilePathResult FilePath(string filePath, string contentType, string contentDisposition, bool clearOutput, bool cacheOutput)
        {
            var result = Rest.Active.CreateObject<FilePathResult>();
            result.FilePath = filePath;
            result.ContentType = contentType;
            result.ContentDisposition = contentDisposition;
            result.ClearOutput = clearOutput;
            result.CacheOutput = cacheOutput;

            return result;
        }

        public static FileUrlResult FileUrl(string fileUrl)
        {
            return FileUrl(fileUrl, null, null, true, true);
        }

        public static FileUrlResult FileUrl(string fileUrl, bool clearOutput)
        {
            return FileUrl(fileUrl, null, null, clearOutput, true);
        }

        public static FileUrlResult FileUrl(string fileUrl, string contentType, bool clearOutput)
        {
            return FileUrl(fileUrl, contentType, null, clearOutput, true);
        }

        public static FileUrlResult FileUrl(string fileUrl, string contentType, bool clearOutput, bool cacheOutput)
        {
            return FileUrl(fileUrl, contentType, null, clearOutput, cacheOutput);
        }

        public static FileUrlResult FileUrl(string fileUrl, string contentType, string contentDisposition, bool clearOutput, bool cacheOutput)
        {
            var result = Rest.Active.CreateObject<FileUrlResult>();
            result.FileUrl = fileUrl;
            result.ContentType = contentType;
            result.ContentDisposition = contentDisposition;
            result.ClearOutput = clearOutput;
            result.CacheOutput = cacheOutput;

            return result;
        }

        public static RedirectResult Redirect(string redirectUrl)
        {
            return Redirect(redirectUrl, false);
        }

        public static RedirectResult Redirect(string redirectUrl, bool isPermanent)
        {
            var result = Rest.Active.CreateObject<RedirectResult>();
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
            var result = Rest.Active.CreateObject<StatusResult>();
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
            var result = Rest.Active.CreateObject<JsonResult>();
            result.Content = obj;

            return result;
        }

        public static XmlResult Xml(object obj)
        {
            return Xml(obj, null);
        }

        public static XmlResult Xml(object obj, Type[] extraTypes)
        {
            var result = Rest.Active.CreateObject<XmlResult>();
            result.Content = obj;
            result.ExtraTypes = extraTypes;

            return result;
        }
    }
}
