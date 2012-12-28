// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.IO;
using Newtonsoft.Json;
using RestFoundation.Results;
using RestFoundation.Runtime;

namespace RestFoundation.Formatters
{
    /// <summary>
    /// Represents a JSONP media type formatter.
    /// </summary>
    [SupportedMediaType("application/javascript")]
    [SupportedMediaType("text/javascript")]
    public class JsonPFormatter : IMediaTypeFormatter
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

            if (context.Request.Body.CanSeek)
            {
                context.Request.Body.Seek(0, SeekOrigin.Begin);
            }

            var streamReader = new StreamReader(context.Request.Body, context.Request.Headers.ContentCharsetEncoding);
            var serializer = JsonSerializerFactory.Create();
            var reader = new JsonTextReader(streamReader);

            if (objectType == typeof(object))
            {
                return serializer.Deserialize(reader);
            }

            return serializer.Deserialize(reader, objectType);
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
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return new JsonPResult
            {
                Callback = context.Request.QueryString.TryGet("callback"),
                Content = obj
            };
        }
    }
}
