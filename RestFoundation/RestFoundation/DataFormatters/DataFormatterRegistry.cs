using System;
using System.Collections.Concurrent;
using System.Linq;

namespace RestFoundation.DataFormatters
{
    internal static class DataFormatterRegistry
    {
        private static readonly ConcurrentDictionary<string, IDataFormatter> contentTypeFormatters = new ConcurrentDictionary<string, IDataFormatter>(StringComparer.OrdinalIgnoreCase);
        private static readonly ConcurrentDictionary<Type, IDataFormatter> resourceTypeFormatters = new ConcurrentDictionary<Type, IDataFormatter>();

        static DataFormatterRegistry()
        {
            contentTypeFormatters.TryAdd("application/json", new JsonFormatter());
            contentTypeFormatters.TryAdd("application/xml", new XmlFormatter());
            contentTypeFormatters.TryAdd("text/xml", new XmlFormatter());
        }

        public static IDataFormatter GetFormatter(string contentType)
        {
            IDataFormatter formatter;

            return contentTypeFormatters.TryGetValue(contentType, out formatter) ? formatter : null;
        }

        public static IDataFormatter GetFormatter(Type resourceType)
        {
            IDataFormatter formatter;

            return resourceTypeFormatters.TryGetValue(resourceType, out formatter) ? formatter : null;
        }

        public static string[] GetContentTypesToFormat()
        {
            return contentTypeFormatters.Keys.ToArray();
        }

        public static void SetFormatter(string contentType, IDataFormatter formatter)
        {
            contentTypeFormatters.AddOrUpdate(contentType, type => formatter, (type, previousFormatter) => formatter);
        }
        
        public static void SetFormatter(Type resourceType, IDataFormatter formatter)
        {
            resourceTypeFormatters.AddOrUpdate(resourceType, type => formatter, (type, previousFormatter) => formatter);
        }

        public static bool RemoveFormatter(string contentType)
        {
            IDataFormatter formatter;

            return contentTypeFormatters.TryRemove(contentType, out formatter);
        }

        public static bool RemoveFormatter(Type resourceType)
        {
            IDataFormatter formatter;

            return resourceTypeFormatters.TryRemove(resourceType, out formatter);
        }

        public static void ClearContentTypeFormatters()
        {
            contentTypeFormatters.Clear();
        }

        public static void ClearResourceTypeFormatters()
        {
            resourceTypeFormatters.Clear();
        }
    }
}
