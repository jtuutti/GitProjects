using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using RestFoundation.Results;

namespace RestFoundation.Formatters
{
    /// <summary>
    /// Represents a BSON content formatter.
    /// </summary>
    public class BsonFormatter : IContentFormatter
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

            using (var reader = new BsonReader(context.Request.Body))
            {
                var serializer = new JsonSerializer();

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

            return new BsonResult
            {
                Content = obj
            };
        }
    }
}
