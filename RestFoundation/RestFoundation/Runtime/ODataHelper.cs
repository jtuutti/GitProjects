// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using RestFoundation.Odata;

namespace RestFoundation.Runtime
{
    internal static class ODataHelper
    {
        public static object PerformOdataOperations(object returnedObj, IHttpRequest request)
        {
            if (returnedObj == null)
            {
                return null;
            }

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

            if (returnItemType.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, RestResources.UnsupportedObjectTypeForOData);
            }

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
