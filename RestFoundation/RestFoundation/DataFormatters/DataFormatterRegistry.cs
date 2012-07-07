using System;
using System.Collections.Concurrent;
using System.Linq;

namespace RestFoundation.DataFormatters
{
    internal static class DataFormatterRegistry
    {
        private static readonly ConcurrentDictionary<string, IDataFormatter> contentTypeFormatters = InitializeDefaultFormatters();

        public static IDataFormatter GetFormatter(string contentType)
        {
            if (String.IsNullOrWhiteSpace(contentType))
            {
                return null;
            }

            IDataFormatter formatter;

            return contentTypeFormatters.TryGetValue(contentType, out formatter) ? formatter : null;
        }

        public static string[] GetContentTypes()
        {
            return contentTypeFormatters.Keys.ToArray();
        }

        public static void SetFormatter(string contentType, IDataFormatter formatter)
        {
            contentTypeFormatters.AddOrUpdate(contentType, type => formatter, (type, previousFormatter) => formatter);
        }

        public static bool RemoveFormatter(string contentType)
        {
            if (String.IsNullOrWhiteSpace(contentType))
            {
                return false;
            }

            IDataFormatter formatter;

            return contentTypeFormatters.TryRemove(contentType, out formatter);
        }

        public static void Clear()
        {
            contentTypeFormatters.Clear();
        }

        private static ConcurrentDictionary<string, IDataFormatter> InitializeDefaultFormatters()
        {
            var defaultFormatters = new ConcurrentDictionary<string, IDataFormatter>(StringComparer.OrdinalIgnoreCase);
            defaultFormatters.TryAdd("application/json", new JsonFormatter());
            defaultFormatters.TryAdd("application/xml", new XmlFormatter());
            defaultFormatters.TryAdd("text/xml", new XmlFormatter());

            return defaultFormatters;
        }
    }
}
