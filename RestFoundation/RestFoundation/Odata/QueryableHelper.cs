using System;
using System.Collections.Specialized;
using System.Linq;
using RestFoundation.Odata.Parser;

namespace RestFoundation.Odata
{
    /// <summary>
    /// Provides the filter method for a queryable sequence of elements.
    /// </summary>
    public static class QueryableHelper
    {
        /// <summary>
        /// Filters a queryable sequence of elements based on ODATA parameters.
        /// </summary>
        /// <returns>The filtered sequence of elements.</returns>
        public static object Filter(object source, NameValueCollection query)
        {
            if (source == null) throw new ArgumentNullException("source");

            if (!source.GetType().IsGenericType || source.GetType().GetGenericTypeDefinition().GetInterface(typeof(IQueryable<>).FullName) == null)
            {
                return source;
            }

            Type modelType = source.GetType().GetGenericArguments()[0];
            Type parserType = typeof(ParameterParser<>).MakeGenericType(modelType);

            object parser = Activator.CreateInstance(parserType);
            
            object filter = parserType.GetMethod("Parse").Invoke(parser, new object[] { query });
            if (filter == null) return source;

            return filter.GetType().GetMethod("Filter").Invoke(filter, new[] { source });
        }
    }
}
