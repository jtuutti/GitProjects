// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RestFoundation.Formatters
{
    internal static class MediaTypeExtractor
    {
        public static HashSet<string> GetMediaTypes<T>()
            where T : class, IMediaTypeFormatter
        {
            var supportedMediaTypes = typeof(T).GetCustomAttributes<SupportedMediaTypeAttribute>(false)
                                               .Select(a => a.MediaType);

            return new HashSet<string>(supportedMediaTypes, StringComparer.OrdinalIgnoreCase);
        }
    }
}
