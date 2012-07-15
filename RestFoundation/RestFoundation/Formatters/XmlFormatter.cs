using System;
using System.IO;
using System.Xml.Serialization;
using RestFoundation.Context;
using RestFoundation.Results;
using RestFoundation.Runtime;

namespace RestFoundation.Formatters
{
    /// <summary>
    /// Represents an XML content formatter.
    /// </summary>
    public class XmlFormatter : IContentFormatter
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

            if (objectType == typeof(object))
            {
                using (var streamReader = new StreamReader(context.Request.Body, context.Request.Headers.ContentCharsetEncoding))
                {
                    return new DynamicXDocument(streamReader.ReadToEnd());
                }
            }

            if (context.Request.Body.CanSeek)
            {
                context.Request.Body.Seek(0, SeekOrigin.Begin);
            }

            XmlSerializer serializer;

            try
            {
                serializer = new XmlSerializer(objectType);
            }
            catch (NotSupportedException) // cannot find a serializer for the type
            {
                return null;
            }

            return serializer.Deserialize(context.Request.Body);
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

            return new XmlResult
            {
                Content = obj,
                ContentType = context.Request.GetPreferredAcceptType()
            };
        }
    }
}
