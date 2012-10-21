// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestFoundation.Formatters;
using RestFoundation.Odata;
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

            if (result != null)
            {
                return result;
            }

            return CreateFormatterResult(returnedObj, methodReturnType, handler);
        }

        private static object PerformOdataOperations(object returnedObj, IHttpRequest request)
        {
            object filteredResults;

            try
            {
                filteredResults = QueryableHelper.Filter(returnedObj, request.QueryString.ToNameValueCollection());
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, RestResources.InvalidODataParameters);
            }

            var filteredResultArray = filteredResults as object[];

            if (filteredResultArray == null || filteredResultArray.Length == 0 || filteredResultArray[0] == null)
            {
                return filteredResults;
            }

            Type returnItemType = filteredResultArray[0].GetType();
            Type filteredResultListType = typeof(List<>).MakeGenericType(returnItemType);

            object filteredResultList = Activator.CreateInstance(filteredResultListType);
            var method = filteredResultListType.GetMethod("Add", new[] { returnItemType });

            foreach (var filteredResult in filteredResultArray)
            {
                method.Invoke(filteredResultList, new[] { filteredResult });
            }

            return filteredResultListType.GetMethod("ToArray").Invoke(filteredResultList, null);
        }

        private IResult CreateFormatterResult(object returnedObj, Type methodReturnType, IRestHandler handler)
        {
            if (returnedObj != null && methodReturnType.IsGenericType && methodReturnType.GetGenericTypeDefinition() == typeof(IQueryable<>))
            {
                returnedObj = PerformOdataOperations(returnedObj, handler.Context.Request);
            }

            string acceptType = m_contentNegotiator.GetPreferredMediaType(handler.Context.Request);

            IMediaTypeFormatter formatter = MediaTypeFormatterRegistry.GetHandlerFormatter(handler, acceptType) ??
                                            MediaTypeFormatterRegistry.GetFormatter(acceptType);

            if (formatter == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable, RestResources.UnsupportedMediaType);
            }

            return formatter.FormatResponse(handler.Context, methodReturnType, returnedObj);
        }
    }
}
