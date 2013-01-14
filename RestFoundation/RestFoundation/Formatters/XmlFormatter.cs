﻿// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using RestFoundation.Results;
using RestFoundation.Runtime;

namespace RestFoundation.Formatters
{
    /// <summary>
    /// Represents an XML media type formatter.
    /// </summary>
    [SupportedMediaType("application/xml")]
    [SupportedMediaType("text/xml")]
    public class XmlFormatter : IMediaTypeFormatter
    {
        private static readonly HashSet<string> supportedMediaTypes = GetSupportedMediaTypes();

        private readonly IContentNegotiator m_contentNegotiator;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlFormatter"/> class with the provided content negotiator.
        /// </summary>
        public XmlFormatter() : this(Rest.Configuration.ServiceLocator.GetService<IContentNegotiator>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlFormatter"/> class.
        /// </summary>
        /// <param name="contentNegotiator">The content negotiator.</param>
        public XmlFormatter(IContentNegotiator contentNegotiator)
        {
            if (contentNegotiator == null)
            {
                throw new ArgumentNullException("contentNegotiator");
            }

            m_contentNegotiator = contentNegotiator;
        }

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

            var reader = XmlReader.Create(new StreamReader(context.Request.Body, context.Request.Headers.ContentCharsetEncoding));

            return XmlSerializerRegistry.Get(objectType).Deserialize(reader);
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

            return new XmlResult
            {
                Content = obj,
                ContentType = GetFormatterMediaType(context.Request),
                ReturnedType = methodReturnType
            };
        }

        private static HashSet<string> GetSupportedMediaTypes()
        {
            var supportedMediaTypeAttributes = typeof(XmlFormatter).GetCustomAttributes(typeof(SupportedMediaTypeAttribute), false).Cast<SupportedMediaTypeAttribute>();

            return new HashSet<string>(supportedMediaTypeAttributes.Select(a => a.MediaType), StringComparer.OrdinalIgnoreCase);
        }

        private string GetFormatterMediaType(IHttpRequest request)
        {
            string preferredMediaType = m_contentNegotiator.GetPreferredMediaType(request);

            return supportedMediaTypes.Contains(preferredMediaType) ? preferredMediaType : supportedMediaTypes.First();
        }
    }
}
