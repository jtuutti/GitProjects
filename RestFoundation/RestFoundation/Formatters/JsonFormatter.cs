using System;
using System.IO;
using Newtonsoft.Json;
using RestFoundation.Context;
using RestFoundation.Results;

namespace RestFoundation.Formatters
{
    /// <summary>
    /// Represents a JSON content type formatter.
    /// </summary>
    public class JsonFormatter : IContentTypeFormatter
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

            if (context.Request.Body.CanSeek)
            {
                context.Request.Body.Seek(0, SeekOrigin.Begin);
            }

            using (var streamReader = new StreamReader(context.Request.Body, context.Request.Headers.ContentCharsetEncoding))
            {
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(streamReader);

                if (objectType == typeof(object))
                {
                    return serializer.Deserialize(reader);
                }

                return serializer.Deserialize(reader, objectType);
            }
        }

        /// <summary>
        /// Serializes the object instance into the HTTP response stream using the accepted content type.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A service method result containing the serialized object representation.</returns>
        /// <exception cref="HttpResponseException">If the object cannot be serialized.</exception>
        public virtual IResult FormatResponse(IServiceContext context, object obj)
        {
            if (context == null) throw new ArgumentNullException("context");

            return new JsonResult
            {
                Content = obj,
                ContentType = context.Request.GetPreferredAcceptType()
            };
        }
    }
}
