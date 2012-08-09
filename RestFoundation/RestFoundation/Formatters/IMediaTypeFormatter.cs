// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using RestFoundation.Results;

namespace RestFoundation.Formatters
{
    /// <summary>
    /// Defines a formatter responsible for serializing and deserializing strongly typed objects
    /// based on the HTTP message body data and associated the media type.
    /// </summary>
    public interface IMediaTypeFormatter
    {
        /// <summary>
        /// Deserializes HTTP message body data into an object instance of the provided type.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="objectType">The object type.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="HttpResponseException">If the object could not be deserialized.</exception>
        object FormatRequest(IServiceContext context, Type objectType);

        /// <summary>
        /// Serializes the object instance into the HTTP response stream using the accepted media type.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="methodReturnType">The method return type.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A service method result containing the serialized object representation.</returns>
        /// <exception cref="HttpResponseException">If the object could not be serialized.</exception>
        IResult FormatResponse(IServiceContext context, Type methodReturnType, object obj);
    }
}
