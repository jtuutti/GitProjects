using System;

namespace RestFoundation.Formatters
{
    /// <summary>
    /// Represents a content formatter to block globally supported content types for specific routes.
    /// </summary>
    public sealed class BlockContentFormatter : IContentFormatter
    {
        /// <summary>
        /// Deserializes HTTP message body data into an object instance of the provided type.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="objectType">The object type.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="HttpResponseException">If the object could not be deserialized.</exception>
        public object FormatRequest(IServiceContext context, Type objectType)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Serializes the object instance into the HTTP response stream using the accepted content type.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A service method result containing the serialized object representation.</returns>
        /// <exception cref="HttpResponseException">If the object could not be serialized.</exception>
        public IResult FormatResponse(IServiceContext context, object obj)
        {
            throw new NotSupportedException();
        }
    }
}
