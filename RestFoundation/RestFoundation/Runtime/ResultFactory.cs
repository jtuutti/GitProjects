using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestFoundation.Context;
using RestFoundation.Odata;

namespace RestFoundation.Runtime
{
    public class ResultFactory : IResultFactory
    {
        public virtual IResult Create(IServiceContext context, object returnedObj)
        {
            if (context == null) throw new ArgumentNullException("context");

            var result = returnedObj as IResult;

            if (result != null)
            {
                return result;
            }

            return CreateFormatterResult(context, returnedObj);
        }

        private static IResult CreateFormatterResult(IServiceContext context, object returnedObj)
        {
            if (returnedObj != null && returnedObj.GetType().IsGenericType && returnedObj.GetType().GetGenericTypeDefinition().GetInterface(typeof(IQueryable<>).FullName) != null)
            {
                returnedObj = PerformOdataOperations(context, returnedObj);
            }

            IDataFormatter formatter = DataFormatterRegistry.GetFormatter(context.Request.GetPreferredAcceptType());

            if (formatter == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable, "No supported content type was provided in the Accept or the Content-Type header");
            }

            return formatter.FormatResponse(context, returnedObj);
        }

        private static object PerformOdataOperations(IServiceContext context, object returnedObj)
        {
            object filteredResults;

            try
            {
                filteredResults = QueryableHelper.Filter(returnedObj, context.Request.QueryString.ToNameValueCollection());
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
