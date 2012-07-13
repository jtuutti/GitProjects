using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RestFoundation.Formatters;

namespace RestFoundation.Runtime
{
    internal static class ContentTypeFormatterRegistry
    {
        private static readonly ConcurrentDictionary<string, IContentTypeFormatter> contentTypeFormatters = InitializeDefaultFormatters();

        public static IContentTypeFormatter GetFormatter(string contentType)
        {
            if (String.IsNullOrWhiteSpace(contentType))
            {
                return null;
            }

            IContentTypeFormatter formatter;

            return contentTypeFormatters.TryGetValue(contentType, out formatter) ? formatter : null;
        }

        public static IList<string> GetContentTypes()
        {
            return contentTypeFormatters.Keys.ToArray();
        }

        public static void SetFormatter(string contentType, IContentTypeFormatter formatter)
        {
            contentTypeFormatters.AddOrUpdate(contentType, type => formatter, (type, previousFormatter) => formatter);
        }

        public static bool RemoveFormatter(string contentType)
        {
            if (String.IsNullOrWhiteSpace(contentType))
            {
                return false;
            }

            IContentTypeFormatter formatter;

            return contentTypeFormatters.TryRemove(contentType, out formatter);
        }

        public static void Clear()
        {
            contentTypeFormatters.Clear();
        }

        private static ConcurrentDictionary<string, IContentTypeFormatter> InitializeDefaultFormatters()
        {
            var defaultFormatters = new ConcurrentDictionary<string, IContentTypeFormatter>(StringComparer.OrdinalIgnoreCase);
            defaultFormatters.TryAdd("application/json", new JsonFormatter());
            defaultFormatters.TryAdd("application/xml", new XmlFormatter());
            defaultFormatters.TryAdd("text/xml", new XmlFormatter());

            return defaultFormatters;
        }
    }
}
