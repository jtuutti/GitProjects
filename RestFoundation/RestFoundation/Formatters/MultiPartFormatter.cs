// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using RestFoundation.Results;
using RestFoundation.Runtime;

namespace RestFoundation.Formatters
{
    /// <summary>
    /// Represents a multi-part media type formatter for uploading files over the HTTP.
    /// </summary>
    [SupportedMediaType("multipart/form-data")]
    public class MultiPartFormatter : IMediaTypeFormatter
    {
        /// <summary>
        /// Deserializes HTTP message body data into an object instance of the provided type.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="objectType">The object type.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="HttpResponseException">If the object cannot be deserialized.</exception>
        public virtual object FormatRequest(IServiceContext context, Type objectType)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (objectType == null)
            {
                throw new ArgumentNullException("objectType");
            }

            if (objectType == typeof(IUploadedFile))
            {
                object uploadedFile;

                if (TryGenerateUploadedFile(context, out uploadedFile))
                {
                    return uploadedFile;
                }
            }

            if (objectType == typeof(IEnumerable<IUploadedFile>) || objectType.GetInterface(typeof(IEnumerable<IUploadedFile>).Name) != null)
            {
                return GenerateUploadedFileCollection(objectType, context);
            }

            throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType, Resources.Global.InvalidUploadedFileType);
        }

        /// <summary>
        /// Serializes the object instance into the HTTP response stream using the accepted media type.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="methodReturnType">The method return type.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A service method result containing the serialized object representation.</returns>
        /// <exception cref="HttpResponseException">If the object could not be serialized.</exception>
        public virtual IResult FormatResponse(IServiceContext context, Type methodReturnType, object obj)
        {
            throw new HttpResponseException(HttpStatusCode.NotAcceptable, Resources.Global.MissingOrInvalidAcceptType);
        }

        private static bool TryGenerateUploadedFile(IServiceContext context, out object uploadedFile)
        {
            HttpFileCollectionBase files = GetFiles(context);
            HttpPostedFileBase file = null;

            foreach (string fileName in files.AllKeys)
            {
                HttpPostedFileBase currentFile = files.Get(fileName);

                if (currentFile == null || String.IsNullOrEmpty(currentFile.FileName))
                {
                    continue;
                }

                file = currentFile;
                break;
            }

            if (file == null)
            {
                uploadedFile = null;
                return false;
            }

            uploadedFile = new UploadedFile(file);
            return true;
        }

        private static object GenerateUploadedFileCollection(Type collectionType, IServiceContext context)
        {
            var fileList = !collectionType.IsInterface ? (ICollection<IUploadedFile>) Activator.CreateInstance(collectionType) : new List<IUploadedFile>();
            HttpFileCollectionBase files = GetFiles(context);
            
            foreach (string fileName in files.AllKeys)
            {
                fileList.Add(new UploadedFile(files.Get(fileName)));
            }

            return fileList;
        }

        private static HttpFileCollectionBase GetFiles(IServiceContext context)
        {
            HttpContextBase httpContext = context.GetHttpContext();

            if (ServiceRequestValidator.IsUnvalidatedRequest(httpContext))
            {
                return httpContext.Request.Unvalidated.Files;
            }

            return httpContext.Request.Files;
        }
    }
}
