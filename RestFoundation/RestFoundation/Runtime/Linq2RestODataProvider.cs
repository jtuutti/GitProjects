// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using Linq2Rest.Parser;
using RestFoundation.Resources;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents the default OData provider for <see cref="T:System.Linq.IQueryable`1"/> collection
    /// results.
    /// </summary>
    public class Linq2RestODataProvider : IODataProvider
    {
        private const string ContentRangeHeader = "Content-Range";
        private const string InlineCountKey = "$inlinecount";
        private const string InlineCountValue = "allpages";
        private const string SkipKey = "$skip";
        private const string TopKey = "$top";

        /// <summary>
        /// Performs a query on a collection and returns the resulting collection of
        /// objects.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="collection">The collection to perform the query on.</param>
        /// <returns>The resulting collection.</returns>
        public virtual IEnumerable PerformQuery(IServiceContext context, IQueryable collection)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (collection == null)
            {
                return null;
            }

            NameValueCollection queryString = context.Request.QueryString.ToNameValueCollection();
            TrySetMaxQueryResults(context, queryString);

            int count;

            List<object> filteredCollection = TryConvertToFilteredCollection(collection, queryString, out count);
            object filteredObject = filteredCollection.FirstOrDefault(o => o != null);
            Type objectType = filteredObject != null ? filteredObject.GetType() : typeof(object);

            if (Attribute.IsDefined(objectType, typeof(CompilerGeneratedAttribute), false))
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, Global.UnsupportedObjectTypeForOData);
            }

            Tuple<int, int> range = GetContentRanges(queryString, count);

            TrySetContentRange(context, filteredCollection, range);

            return GenerateFilteredCollection(filteredCollection, objectType);
        }

        private static Tuple<int, int> GetContentRanges(NameValueCollection queryString, int count)
        {
            int take, skip;

            if (!Int32.TryParse(queryString.Get(TopKey) ?? "0", out take) || take < 0)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, String.Format(CultureInfo.InvariantCulture, Global.InvalidODataPagingParameter, TopKey));
            }

            if (!Int32.TryParse(queryString.Get(SkipKey) ?? "0", out skip) || skip < 0)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, String.Format(CultureInfo.InvariantCulture, Global.InvalidODataPagingParameter, SkipKey));
            }

            if (take == 0 && skip == 0)
            {
                return null;
            }

            return Tuple.Create(skip, count);
        }

        private static void TrySetContentRange(IServiceContext context, List<object> filteredCollection, Tuple<int, int> range)
        {
            if (filteredCollection.Count == 0 || range == null)
            {
                return;
            }

            context.Response.SetHeader(ContentRangeHeader, String.Format(CultureInfo.InvariantCulture,
                                                                         "results {0}-{1}/{2}",
                                                                         range.Item1 + 1,
                                                                         range.Item1 + filteredCollection.Count,
                                                                         range.Item2 < 0 ? "*" : range.Item2.ToString(CultureInfo.InvariantCulture)));
        }

        private static void TrySetMaxQueryResults(IServiceContext context, NameValueCollection queryString)
        {
            object maxResultString = context.GetHttpContext().Items[ServiceCallConstants.MaxQueryResults] ??
                                     Rest.Configuration.Options.ODataSettings.MaxResults.ToString(CultureInfo.InvariantCulture);

            int maxQueryResults = Convert.ToInt32(maxResultString, CultureInfo.InvariantCulture);

            if (maxQueryResults < 0)
            {
                if (!String.IsNullOrEmpty(queryString[TopKey]))
                {
                    return;
                }

                queryString[TopKey] = (maxQueryResults * -1).ToString(CultureInfo.InvariantCulture);
            }
            else if (maxQueryResults > 0)
            {
                string topValue = queryString[TopKey];

                if (!String.IsNullOrEmpty(topValue))
                {
                    int top;

                    if (!Int32.TryParse(topValue, out top))
                    {
                        return;
                    }

                    if (top > 0 && top < maxQueryResults)
                    {
                        maxQueryResults = top;
                    }
                }

                queryString[TopKey] = maxQueryResults.ToString(CultureInfo.InvariantCulture);
            }
        }

        private static List<object> TryConvertToFilteredCollection(IQueryable collection, NameValueCollection queryString, out int returnedTotal)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (!collection.GetType().IsGenericType || collection.GetType().GetGenericTypeDefinition().GetInterface(typeof(IQueryable<>).FullName) == null)
            {
                collection = collection.Cast<object>();
            }

            try
            {
                Type modelType = collection.GetType().GetGenericArguments()[0];
                Type parserType = typeof(ParameterParser<>).MakeGenericType(modelType);

                var parser = Activator.CreateInstance(parserType);
                var filter = parserType.GetMethod("Parse").Invoke(parser, new object[] { queryString });
                var filteredCollection = filter != null ? filter.GetType().GetMethod("Filter").Invoke(filter, new object[] { collection }) : collection;

                if (!String.Equals(InlineCountValue, queryString.Get(InlineCountKey), StringComparison.OrdinalIgnoreCase))
                {
                    returnedTotal = -1;
                }
                else if (queryString.Get(TopKey) == null && queryString.Get(SkipKey) == null)
                {
                    returnedTotal = Queryable.Count((dynamic) filteredCollection);
                }
                else
                {
                    var totalQueryString = new NameValueCollection(queryString);
                    totalQueryString.Remove(TopKey);
                    totalQueryString.Remove(SkipKey);

                    var totalFilter = parserType.GetMethod("Parse").Invoke(parser, new object[] { totalQueryString });
                    var totalFilteredCollection = filter != null ? filter.GetType().GetMethod("Filter").Invoke(totalFilter, new object[] { collection }) : collection;
                    returnedTotal = Queryable.Count((dynamic) totalFilteredCollection);
                }

                return new List<object>((IQueryable<object>) filteredCollection);
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, Global.InvalidODataParameters);
            }
        }

        private static IEnumerable GenerateFilteredCollection(IEnumerable<object> filteredCollection, Type objectType)
        {
            Type filteredListType = typeof(List<>).MakeGenericType(objectType);
            object filteredList = Activator.CreateInstance(filteredListType);

            MethodInfo method = filteredListType.GetMethod("Add", new[] { objectType });

            try
            {
                foreach (var filteredObject in filteredCollection)
                {
                    method.Invoke(filteredList, new[] { filteredObject });
                }
            }
            catch (ArgumentNullException)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, Global.NullODataPropertyReference);
            }

            return (IEnumerable) filteredListType.GetMethod("ToArray").Invoke(filteredList, null);
        }
    }
}
