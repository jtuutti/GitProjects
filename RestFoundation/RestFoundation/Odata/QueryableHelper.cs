using System;
using System.Collections.Specialized;
using System.Linq;
using RestFoundation.Odata.Parser;

namespace RestFoundation.Odata
{
    internal static class QueryableHelper
    {
        public static object Filter(object source, NameValueCollection query)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (!source.GetType().IsGenericType || source.GetType().GetGenericTypeDefinition().GetInterface(typeof(IQueryable<>).FullName) == null)
            {
                return source;
            }

            Type modelType = source.GetType().GetGenericArguments()[0];
            Type parserType = typeof(ParameterParser<>).MakeGenericType(modelType);

            object parser = Activator.CreateInstance(parserType);
            object filter = parserType.GetMethod("Parse").Invoke(parser, new object[] { query });

            return filter != null ? filter.GetType().GetMethod("Filter").Invoke(filter, new[] { source }) : source;
        }
    }
}
