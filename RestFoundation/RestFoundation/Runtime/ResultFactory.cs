using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestFoundation.Odata;

namespace RestFoundation.Runtime
{
    public class ResultFactory : IResultFactory
    {
        private readonly IHttpRequest m_request;

        public ResultFactory(IHttpRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");

            m_request = request;
        }

        public IResult Create(object returnedObj)
        {
            if (returnedObj == null)
            {
                return null;
            }

            var result = returnedObj as IResult;

            if (result != null)
            {
                return result;
            }

            return CreateSerializerResult(returnedObj);
        }

        private object PerformOdataOperations(object returnedObj)
        {
            object filteredResults;

            try
            {
                filteredResults = QueryableHelper.Filter(returnedObj, m_request.QueryString.ToNameValueCollection());
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

        private IResult CreateSerializerResult(object returnedObj)
        {
            string acceptedType = m_request.GetPreferredAcceptType();

            if (returnedObj.GetType().IsGenericType && returnedObj.GetType().GetGenericTypeDefinition().GetInterface(typeof(IQueryable<>).FullName) != null)
            {
                returnedObj = PerformOdataOperations(returnedObj);
            }

            if (String.Equals("application/json", acceptedType, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Json(returnedObj);
            }

            if (String.Equals("application/xml", acceptedType, StringComparison.OrdinalIgnoreCase) ||
                String.Equals("text/xml", acceptedType, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Xml(returnedObj);
            }

            throw new HttpResponseException(HttpStatusCode.NotAcceptable, "No supported content type was provided in the Accept or the Content-Type header");
        }
    }
}
