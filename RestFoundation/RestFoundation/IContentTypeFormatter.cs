﻿using System;

namespace RestFoundation
{
    /// <summary>
    /// Defines a content type formatter responsible for serializing and deserializing strongly typed objects
    /// based on the HTTP message body data and associated the content type.
    /// </summary>
    public interface IContentTypeFormatter
    {
        /// <summary>
        /// Deserializes HTTP message body data into an object instance of the provided type.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="objectType">The object type.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="HttpResponseException">If the object cannot be deserialized.</exception>
        object FormatRequest(IServiceContext context, Type objectType);

        /// <summary>
        /// Serializes the object instance into the HTTP response stream using the accepted content type.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A service method result containing the serialized object representation.</returns>
        /// <exception cref="HttpResponseException">If the object cannot be serialized.</exception>
        IResult FormatResponse(IServiceContext context, object obj);
    }
}
