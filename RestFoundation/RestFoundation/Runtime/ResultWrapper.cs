// <copyright>
// Dmitry Starosta, 2012-2013
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
        /// <param name="returnedObj">The returned object.</param>
        /// <param name="methodReturnType">The method return type.</param>
        /// <param name="handler">The service context handler.</param>
        /// <returns>The wrapper result.</returns>
        public virtual IResult Wrap(object returnedObj, Type methodReturnType, IServiceContextHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            var result = returnedObj as IResult;

            return result ?? CreateFormatterResult(returnedObj, methodReturnType, handler);
        }

        private static IMediaTypeFormatter TryNegotiateMediaType(IServiceContextHandler handler)
        {
            if (handler.Context.Request.Headers.AcceptTypes == null)
            {
                return null;
            }

            foreach (string acceptType in handler.Context.Request.Headers.AcceptTypes)
            {
                IMediaTypeFormatter formatter = MediaTypeFormatterRegistry.GetHandlerFormatter(handler, acceptType) ??
                                                MediaTypeFormatterRegistry.GetFormatter(acceptType);

                if (formatter != null)
                {
                    return formatter;
                }
            }

            return null;
        }

        private IResult CreateFormatterResult(object returnedObj, Type methodReturnType, IServiceContextHandler handler)
        {
            string acceptType = m_contentNegotiator.GetPreferredMediaType(handler.Context.Request);

            IMediaTypeFormatter formatter = MediaTypeFormatterRegistry.GetHandlerFormatter(handler, acceptType) ??
                                            MediaTypeFormatterRegistry.GetFormatter(acceptType) ??
                                            TryNegotiateMediaType(handler);

            if (formatter == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable, RestResources.MissingOrInvalidAcceptType);
            }

            return formatter.FormatResponse(handler.Context, methodReturnType, returnedObj);
        }
    }
}
