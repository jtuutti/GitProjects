﻿using System;
using System.IO;
using System.Runtime.Serialization.Json;
using RestFoundation.Context;
using RestFoundation.Results;

namespace RestFoundation.DataFormatters
{
    /// <summary>
    /// Represents a data contract serializer based JSON formatter.
    /// </summary>
    public class DataContractJsonFormatter : IContentFormatter
    {
        /// <summary>
        /// Deserializes HTTP message body data into an object instance of the provided type.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="objectType">The object type.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="HttpResponseException">If the object could not be deserialized.</exception>
        public virtual object FormatRequest(IServiceContext context, Type objectType)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (objectType == null) throw new ArgumentNullException("objectType");

            if (context.Request.Body.CanSeek)
            {
                context.Request.Body.Seek(0, SeekOrigin.Begin);
            }

            var serializer = new DataContractJsonSerializer(objectType);

            return serializer.ReadObject(context.Request.Body);
        }

        /// <summary>
        /// Serializes the object instance into the HTTP response stream using the accepted content type.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A service method result containing the serialized object representation.</returns>
        /// <exception cref="HttpResponseException">If the object could not be serialized.</exception>
        public virtual IResult FormatResponse(IServiceContext context, object obj)
        {
            if (context == null) throw new ArgumentNullException("context");

            return new DataContractJsonResult
            {
                Content = obj,
                ContentType = context.Request.GetPreferredAcceptType()
            };
        }
    }
}