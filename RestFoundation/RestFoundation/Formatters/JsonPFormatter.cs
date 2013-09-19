// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestFoundation.Resources;
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
        private static readonly HashSet<string> supportedMediaTypes = MediaTypeExtractor.GetMediaTypes<JsonPFormatter>();

        /// <summary>
        /// Gets a value indicating whether the formatter can format message body in HTTP
        /// requests.
        /// </summary>
        public virtual bool CanFormatRequest
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
        public virtual bool CanFormatResponse
        {
            get
            {
                return true;
            }
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
        /// <exception cref="HttpResponseException">If the object could not be serialized.</exception>
        public virtual IResult FormatResponse(IServiceContext context, Type methodReturnType, object obj, string preferredMediaType)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var result = new JsonResult
            {
                Callback = context.Request.QueryString.TryGet("callback") ?? context.Request.QueryString.TryGet("$callback"),
                Content = obj,
                ContentType = preferredMediaType != null && supportedMediaTypes.Contains(preferredMediaType) ? preferredMediaType : supportedMediaTypes.First(),
                ReturnedType = methodReturnType
            };

            if (String.IsNullOrWhiteSpace(result.Callback))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, Global.InvalidJsonPCallback);
            }

            return result;
        }
    }
}
