using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestFoundation.Context;
using RestFoundation.Odata;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents the default result factory that converts POCO objects into
    /// results using content formatters.
    /// </summary>
    public class ResultFactory : IResultFactory
    {
        /// <summary>
        /// Creates an <see cref="IResult"/> instance from a POCO object returned by
        /// the service method.
        /// </summary>
        /// <param name="returnedObj">The returned object.</param>
        /// <param name="handler">The REST handler.</param>
        /// <returns>The created result instance.</returns>
        public virtual IResult Create(object returnedObj, IRestHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            var result = returnedObj as IResult;

            if (result != null)
            {
                return result;
            }

            return CreateFormatterResult(returnedObj, handler);
        }

        private static IResult CreateFormatterResult(object returnedObj, IRestHandler handler)
        {
            if (returnedObj != null && returnedObj.GetType().IsGenericType && returnedObj.GetType().GetGenericTypeDefinition().GetInterface(typeof(IQueryable<>).FullName) != null)
            {
                returnedObj = PerformOdataOperations(returnedObj, handler.Context.Request);
            }

            string acceptType = handler.Context.Request.GetPreferredAcceptType();

            IContentFormatter formatter = ContentFormatterRegistry.GetHandlerFormatter(handler, acceptType) ??
                                          ContentFormatterRegistry.GetFormatter(acceptType);

            if (formatter == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable, "No supported content type was provided in the Accept or the Content-Type header");
            }

            return formatter.FormatResponse(handler.Context, returnedObj);
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
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid OData parameters provided");
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
    }
}
