// <copyright>
// Dmitry Starosta, 2012-2013
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
using RestFoundation.Collections;

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

            var range = GetContentRanges(context.Request.QueryString, collection);

            List<object> filteredCollection = TryConvertToFilteredCollection(collection, context.Request.QueryString.ToNameValueCollection());
            object filteredObject = filteredCollection.FirstOrDefault(o => o != null);
            Type objectType = filteredObject != null ? filteredObject.GetType() : typeof(object);
                
            if (objectType.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, Resources.Global.UnsupportedObjectTypeForOData);
            }

            TrySetContentRange(context, filteredCollection, range);

            return GenerateFilteredCollection(filteredCollection, objectType);
        }

        private static Tuple<int, int> GetContentRanges(IStringValueCollection queryString, IQueryable collection)
        {
            if (!String.Equals(InlineCountValue, queryString.TryGet(InlineCountKey), StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            int skip, count = Queryable.Count((dynamic) collection);

            if (!Int32.TryParse(queryString.TryGet(SkipKey), out skip) || skip < 0)
            {
                skip = 0;
            }

            return Tuple.Create(skip + 1, count);
        }

        private static void TrySetContentRange(IServiceContext context, List<object> filteredCollection, Tuple<int, int> range)
        {
            if (filteredCollection.Count == 0 || range == null)
            {
                return;
            }

            context.Response.SetHeader(ContentRangeHeader, String.Format(CultureInfo.InvariantCulture,
                                                                         "results {0}-{1}/{2}",
                                                                         range.Item1,
                                                                         range.Item1 + filteredCollection.Count - 1,
                                                                         range.Item2));
        }

        private static List<object> TryConvertToFilteredCollection(IQueryable collection, NameValueCollection queryString)
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

                return new List<object>((IQueryable<object>) filteredCollection);
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, Resources.Global.InvalidODataParameters);
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
                throw new HttpResponseException(HttpStatusCode.BadRequest, Resources.Global.NullODataPropertyReference);
            }

            return (IEnumerable) filteredListType.GetMethod("ToArray").Invoke(filteredList, null);
        }
    }
}
