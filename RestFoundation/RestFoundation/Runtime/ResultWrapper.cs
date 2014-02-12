// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Net;
using RestFoundation.Formatters;
using RestFoundation.Results;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents the default result wrapper that converts POCO objects into
    /// results using the registered media type formatters.
    /// </summary>
    public class ResultWrapper : IResultWrapper
    {
        private readonly IContentNegotiator m_contentNegotiator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultWrapper"/> class with the provided content negotiator.
        /// </summary>
        /// <param name="contentNegotiator">The content negotiator.</param>
        public ResultWrapper(IContentNegotiator contentNegotiator)
        {
            if (contentNegotiator == null)
            {
                throw new ArgumentNullException("contentNegotiator");
            }

            m_contentNegotiator = contentNegotiator;
        }

        /// <summary>
        /// Wraps a POCO object returned by the service method with an <see cref="IResult"/>.
        /// </summary>
        /// <param name="handler">The service context handler.</param>
        /// <param name="returnedObj">The returned object.</param>
        /// <param name="methodReturnType">The method return type.</param>
        /// <returns>The wrapper result.</returns>
        public virtual IResult Wrap(IServiceContextHandler handler, object returnedObj, Type methodReturnType)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            var result = returnedObj as IResult;

            return result ?? CreateFormatterResult(handler, returnedObj, methodReturnType);
        }

        private IResult CreateFormatterResult(IServiceContextHandler handler, object returnedObj, Type methodReturnType)
        {
            string preferredMediaType = m_contentNegotiator.GetPreferredMediaType(handler.Context.Request);

            IMediaTypeFormatter formatter = MediaTypeFormatterRegistry.GetHandlerFormatter(handler, preferredMediaType) ??
                                            MediaTypeFormatterRegistry.GetFormatter(preferredMediaType);

            if (formatter == null || !formatter.CanFormatResponse)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable, Resources.Global.MissingOrInvalidAcceptType);
            }

            return formatter.FormatResponse(handler.Context, methodReturnType, returnedObj, preferredMediaType);
        }
    }
}
