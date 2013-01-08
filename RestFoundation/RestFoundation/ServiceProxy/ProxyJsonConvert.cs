// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using Newtonsoft.Json;
using JsonConvertInternal = Newtonsoft.Json.JsonConvert;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents an JSON object converter.
    /// </summary>
    public static class ProxyJsonConvert
    {
        /// <summary>
        /// Converts an object into JSON.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <returns>The <see cref="String"/> value containing the serialized object.</returns>
        public static string SerializeObject(object value)
        {
            return SerializeObject(value, false);
        }

        /// <summary>
        /// Converts an object into JSON with the provided formatting options.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="isFormatted">
        /// A <see cref="bool"/> indicating whether the serialized object is formatted for output.
        /// </param>
        /// <returns>The <see cref="String"/> value containing the serialized object.</returns>
        public static string SerializeObject(object value, bool isFormatted)
        {
            var options = Rest.Configuration.Options.JsonSettings.ToJsonSerializerSettings();

            if (isFormatted)
            {
                options.Formatting = Formatting.Indented;
            }

            return JsonConvertInternal.SerializeObject(value, options);
        }
    }
}
