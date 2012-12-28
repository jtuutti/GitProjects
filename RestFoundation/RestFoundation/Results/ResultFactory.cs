// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Net;
using RestFoundation.Formatters;
using RestFoundation.Runtime;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents the default result factory that converts POCO objects into
    /// results using the registered media type formatters.
    /// </summary>
    public class ResultFactory : IResultFactory
    {
        private readonly IContentNegotiator m_contentNegotiator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultFactory"/> class with the provided content negotiator.
        /// </summary>
        /// <param name="contentNegotiator">The content negotiator.</param>
        public ResultFactory(IContentNegotiator contentNegotiator)
        {
            if (contentNegotiator == null)
            {
                throw new ArgumentNullException("contentNegotiator");
            }

            m_contentNegotiator = contentNegotiator;
        }

        /// <summary>
        /// Creates an <see cref="IResult"/> instance from a POCO object returned by the service method.
        /// </summary>
        /// <param name="returnedObj">The returned object.</param>
        /// <param name="methodReturnType">The method return type.</param>
        /// <param name="handler">The REST handler.</param>
        /// <returns>The created result instance.</returns>
        public virtual IResult Create(object returnedObj, Type methodReturnType, IRestHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            var result = returnedObj as IResult;

            return result ?? CreateFormatterResult(returnedObj, methodReturnType, handler);
        }

        private IResult CreateFormatterResult(object returnedObj, Type methodReturnType, IRestHandler handler)
        {
            string acceptType = m_contentNegotiator.GetPreferredMediaType(handler.Context.Request);

            IMediaTypeFormatter formatter = MediaTypeFormatterRegistry.GetHandlerFormatter(handler, acceptType) ??
                                            MediaTypeFormatterRegistry.GetFormatter(acceptType);

            if (formatter == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable, RestResources.MissingValidMediaType);
            }

            return formatter.FormatResponse(handler.Context, methodReturnType, returnedObj);
        }
    }
}
