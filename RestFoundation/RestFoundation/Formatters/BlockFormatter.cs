// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using RestFoundation.Results;

namespace RestFoundation.Formatters
{
    /// <summary>
    /// Represents a media type formatter to block globally supported media types for specific routes.
    /// </summary>
    public sealed class BlockFormatter : IMediaTypeFormatter
    {
        /// <summary>
        /// Gets a value indicating whether the formatter can format message body in HTTP
        /// requests.
        /// </summary>
        public bool CanFormatRequest
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the formatter can format objects returned by service
        /// methods into HTTP response.
        /// </summary>
        public bool CanFormatResponse
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Deserializes HTTP message body data into an object instance of the provided type.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="objectType">The object type.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="RestFoundation.Runtime.HttpResponseException">
        /// If the object could not be serialized.
        /// </exception>
        public object FormatRequest(IServiceContext context, Type objectType)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Serializes the object instance into the HTTP response stream using the accepted media type.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="methodReturnType">The method return type.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="preferredMediaType">The preferred media type.</param>
        /// <returns>A service method result containing the serialized object representation.</returns>
        /// <exception cref="RestFoundation.Runtime.HttpResponseException">
        /// If the object could not be serialized.
        /// </exception>
        public IResult FormatResponse(IServiceContext context, Type methodReturnType, object obj, string preferredMediaType)
        {
            throw new NotSupportedException();
        }
    }
}
