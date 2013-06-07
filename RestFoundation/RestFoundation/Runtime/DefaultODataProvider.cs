// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using RestFoundation.Odata;
using RestFoundation.Results;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents the default OData provider for <see cref="T:System.Linq.IQueryable`1"/> collection
    /// results.
    /// </summary>
    public class DefaultODataProvider : IODataProvider
    {
        /// <summary>
        /// Performs a query on a collection and returns the resulting collection of
        /// objects.
        /// </summary>
        /// <param name="collection">The collection to perform the query on.</param>
        /// <param name="queryString">
        /// A <see cref="NameValueCollection"/> containing the HTTP request query string parameters.
        /// </param>
        /// <returns>The resulting collection.</returns>
        public IEnumerable PerformQuery(IQueryable collection, NameValueCollection queryString)
        {
            if (queryString == null)
            {
                throw new ArgumentNullException("queryString");
            }

            if (collection == null)
            {
                return null;
            }

            IEnumerable filteredCollection = TryConvertToFilteredCollection(collection, queryString);

            var filteredObjectArray = filteredCollection as object[];

            if (filteredObjectArray == null || filteredObjectArray.Length == 0 || filteredObjectArray[0] == null)
            {
                return filteredCollection;
            }

            Type objectType = filteredObjectArray[0].GetType();

            if (objectType.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, Resources.Global.UnsupportedObjectTypeForOData);
            }

            return GenerateFilteredCollection(objectType, filteredObjectArray);
        }

        private static IEnumerable TryConvertToFilteredCollection(IQueryable collection, NameValueCollection queryString)
        {
            object filteredCollection;

            try
            {
                filteredCollection = QueryableHelper.Filter(collection, queryString);
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, Resources.Global.InvalidODataParameters);
            }

            return (IEnumerable) filteredCollection;
        }

        private static IEnumerable GenerateFilteredCollection(Type objectType, IEnumerable<object> filteredObjectArray)
        {
            Type filteredObjectType = typeof(List<>).MakeGenericType(objectType);

            object filteredCollection = Activator.CreateInstance(filteredObjectType);
            var method = filteredObjectType.GetMethod("Add", new[] { objectType });

            foreach (var filteredObject in filteredObjectArray)
            {
                method.Invoke(filteredCollection, new[] { filteredObject });
            }

            return (IEnumerable) filteredObjectType.GetMethod("ToArray").Invoke(filteredCollection, null);
        }
    }
}
