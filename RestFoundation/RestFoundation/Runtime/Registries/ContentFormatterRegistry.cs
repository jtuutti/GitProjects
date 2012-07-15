using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RestFoundation.Formatters;

namespace RestFoundation.Runtime
{
    internal static class ContentFormatterRegistry
    {
        private static readonly ConcurrentDictionary<string, IContentFormatter> contentFormatters = InitializeDefaultFormatters();

        public static IContentFormatter GetFormatter(string contentType)
        {
            if (String.IsNullOrWhiteSpace(contentType))
            {
                return null;
            }

            IContentFormatter formatter;

            return contentFormatters.TryGetValue(contentType, out formatter) ? formatter : null;
        }

        public static IList<string> GetContentTypes()
        {
            return contentFormatters.Keys.ToArray();
        }

        public static void SetFormatter(string contentType, IContentFormatter formatter)
        {
            contentFormatters.AddOrUpdate(contentType, type => formatter, (type, previousFormatter) => formatter);
        }

        public static bool RemoveFormatter(string contentType)
        {
            if (String.IsNullOrWhiteSpace(contentType))
            {
                return false;
            }

            IContentFormatter formatter;

            return contentFormatters.TryRemove(contentType, out formatter);
        }

        public static void Clear()
        {
            contentFormatters.Clear();
        }

        private static ConcurrentDictionary<string, IContentFormatter> InitializeDefaultFormatters()
        {
            var defaultFormatters = new ConcurrentDictionary<string, IContentFormatter>(StringComparer.OrdinalIgnoreCase);
            defaultFormatters.TryAdd("application/json", new JsonFormatter());
            defaultFormatters.TryAdd("application/xml", new XmlFormatter());
            defaultFormatters.TryAdd("text/xml", new XmlFormatter());

            return defaultFormatters;
        }
    }
}
