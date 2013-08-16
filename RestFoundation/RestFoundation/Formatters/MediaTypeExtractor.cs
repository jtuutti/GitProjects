// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestFoundation.Formatters
{
    internal static class MediaTypeExtractor
    {
        public static HashSet<string> GetMediaTypes<T>()
            where T : class, IMediaTypeFormatter
        {
            var supportedMediaTypeAttributes = typeof(T).GetCustomAttributes(typeof(SupportedMediaTypeAttribute), false).Cast<SupportedMediaTypeAttribute>();

            return new HashSet<string>(supportedMediaTypeAttributes.Select(a => a.MediaType), StringComparer.OrdinalIgnoreCase);
        }
    }
}
