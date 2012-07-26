﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RestFoundation.Formatters;

namespace RestFoundation.Runtime
{
    internal static class MediaTypeFormatterRegistry
    {
        private static readonly ConcurrentDictionary<string, IMediaTypeFormatter> formatters = InitializeDefaultFormatters();
        private static readonly ConcurrentDictionary<IRestHandler, Dictionary<string, IMediaTypeFormatter>> handlerFormatters = new ConcurrentDictionary<IRestHandler, Dictionary<string, IMediaTypeFormatter>>();

        public static IMediaTypeFormatter GetFormatter(string mediaType)
        {
            if (String.IsNullOrWhiteSpace(mediaType))
            {
                return null;
            }

            IMediaTypeFormatter formatter;

            return formatters.TryGetValue(mediaType, out formatter) ? formatter : null;
        }

        public static IList<string> GetMediaTypes()
        {
            return formatters.Keys.ToArray();
        }

        public static void SetFormatter(string mediaType, IMediaTypeFormatter formatter)
        {
            formatters.AddOrUpdate(mediaType, type => formatter, (type, previousFormatter) => formatter);
        }

        public static bool RemoveFormatter(string mediaType)
        {
            if (String.IsNullOrWhiteSpace(mediaType))
            {
                return false;
            }

            IMediaTypeFormatter formatter;

            return formatters.TryRemove(mediaType, out formatter);
        }

        public static void Clear()
        {
            formatters.Clear();
        }

        public static IMediaTypeFormatter GetHandlerFormatter(IRestHandler handler, string mediaType)
        {
            Dictionary<string, IMediaTypeFormatter> formatterDictionary;

            if (!handlerFormatters.TryGetValue(handler, out formatterDictionary))
            {
                return null;
            }

            IMediaTypeFormatter formatter;

            return formatterDictionary.TryGetValue(mediaType, out formatter) ? formatter : null;
        }

        public static void AddHandlerFormatter(IRestHandler handler, string mediaType, IMediaTypeFormatter formatter)
        {
            handlerFormatters.AddOrUpdate(handler,
                                          handlerToAdd =>
                                          {
                                              var formattersToAdd = new Dictionary<string, IMediaTypeFormatter>(StringComparer.OrdinalIgnoreCase)
                                              {
                                                  { mediaType, formatter }
                                              };

                                              return formattersToAdd;
                                          },
                                          (handlerToUpdate, formattersToUpdate) =>
                                          {
                                              formattersToUpdate[mediaType] = formatter;

                                              return formattersToUpdate;
                                          });
        }

        private static ConcurrentDictionary<string, IMediaTypeFormatter> InitializeDefaultFormatters()
        {
            var defaultFormatters = new ConcurrentDictionary<string, IMediaTypeFormatter>(StringComparer.OrdinalIgnoreCase);
            defaultFormatters.TryAdd("application/json", new JsonFormatter());
            defaultFormatters.TryAdd("application/xml", new XmlFormatter());
            defaultFormatters.TryAdd("text/xml", new XmlFormatter());

            return defaultFormatters;
        }
    }
}