using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace RestFoundation.DataFormatters
{
    public static class Formatters
    {
        private static readonly Dictionary<string, IDataFormatter> formatters = new Dictionary<string, IDataFormatter>(StringComparer.OrdinalIgnoreCase)
        {
            { "application/json", new JsonFormatter() },
            { "application/x-www-form-urlencoded", new FormsFormatter() },
            { "application/xml", new XmlFormatter() },
            { "text/xml", new XmlFormatter() },
        };

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
                         Justification = "Content type should be returned in lower case")]
        public static IDataFormatter GetFormatter(string contentType)
        {
            IDataFormatter formatter;

            if (contentType == null || !formatters.TryGetValue(contentType, out formatter))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType, "No supported content type was provided in the Content-Type header");
            }

            return formatter;
        }
    }
}
