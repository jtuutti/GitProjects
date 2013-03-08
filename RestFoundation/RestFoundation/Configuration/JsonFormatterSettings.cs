// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using Newtonsoft.Json;

namespace RestFoundation.Configuration
{
    /// <summary>
    /// Contains settings used by JSON formatters and results.
    /// </summary>
    public sealed class JsonFormatterSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFormatterSettings"/> class.
        /// </summary>
        public JsonFormatterSettings()
        {
            LowerPropertiesForAjax = true;
        }

        /// <summary>
        /// Gets or sets an optional maximum depth of the object graph.
        /// </summary>
        public int? MaxDepth { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that properties with null values must be serialized.
        /// This property is set to false by default.
        /// </summary>
        public bool IncludeNullValues { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that properties should be in lower case for AJAX
        /// requests.
        /// </summary>
        public bool LowerPropertiesForAjax { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that dates must be serialized in the local server
        /// timezone instead of UTC. This property is set to true by default.
        /// </summary>
        public bool UseLocalTimeZone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that dates must be serialized in Microsoft format
        /// instead of the ISO 8601 format. This property is set to false by default.
        /// </summary>
        public bool UseMicrosoftStyleDates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to wrap a JSON response.
        /// </summary>
        public bool WrapContentResponse { get; set; }

        internal JsonSerializerSettings ToJsonSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                MaxDepth = MaxDepth,
                DateFormatHandling = UseMicrosoftStyleDates ? DateFormatHandling.MicrosoftDateFormat : DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = UseLocalTimeZone ? DateTimeZoneHandling.Local : DateTimeZoneHandling.Utc,
                NullValueHandling = IncludeNullValues ? NullValueHandling.Include : NullValueHandling.Ignore
            };
        }
    }
}
