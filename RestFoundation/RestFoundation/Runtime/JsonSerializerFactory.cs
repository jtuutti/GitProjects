// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using Newtonsoft.Json;
using RestFoundation.Configuration;

namespace RestFoundation.Runtime
{
    internal static class JsonSerializerFactory
    {
        public static JsonSerializer Create()
        {
            JsonFormatterSettings options = Rest.Configuration.Options.JsonSettings;

            var serializer = new JsonSerializer
            {
                Formatting = Formatting.None,
                MaxDepth = options.MaxDepth,
                DateFormatHandling = options.UseMicrosoftStyleDates ? DateFormatHandling.MicrosoftDateFormat : DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = options.UseLocalTimeZone ? DateTimeZoneHandling.Local : DateTimeZoneHandling.Utc,
                NullValueHandling = options.IncludeNullValues ? NullValueHandling.Include : NullValueHandling.Ignore
            };

            return serializer;
        }
    }
}
