using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Web;
using RestFoundation.Runtime;

namespace RestFoundation.Formatters
{
    /// <summary>
    /// Represents a multi-part media type formatter for uploading files over HTTP.
    /// </summary>
    public class MultipartFormatter : IMediaTypeFormatter
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
            if (context == null) throw new ArgumentNullException("context");
            if (objectType == null) throw new ArgumentNullException("objectType");

            if (objectType != typeof(IEnumerable<IUploadedFile>) && objectType != typeof(ICollection<IUploadedFile>))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType, "Object type must be IEnumerable<IUploadedFile> or ICollection<IUploadedFile> for the media type");
            }

            var fileList = new List<IUploadedFile>();
            HttpContextBase httpContext = context.GetHttpContext();

            foreach (string fileName in httpContext.Request.Files.AllKeys)
            {
                fileList.Add(new UploadedFile(httpContext.Request.Files.Get(fileName)));
            }

            return new ReadOnlyCollection<IUploadedFile>(fileList);
        }

        /// <summary>
        /// Serializes the object instance into the HTTP response stream using the accepted media type.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A service method result containing the serialized object representation.</returns>
        /// <exception cref="HttpResponseException">If the object cannot be serialized.</exception>
        public virtual IResult FormatResponse(IServiceContext context, object obj)
        {
            throw new HttpResponseException(HttpStatusCode.NotAcceptable, "No supported media type was provided in the Accept or the Content-Type header");
        }
    }
}
