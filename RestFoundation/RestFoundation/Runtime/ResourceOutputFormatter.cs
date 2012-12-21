// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Globalization;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents a resource output formatter that adds whitespace to improve JSON/XML readability.
    /// </summary>
    public static class ResourceOutputFormatter
    {
        /// <summary>
        /// Formats the input JSON with whitespace.
        /// </summary>
        /// <param name="input">The input JSON string</param>
        /// <returns>The formatted JSON output.</returns>
        public static string FormatJson(string input)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            try
            {
                var options = Rest.Configuration.Options.JsonSettings.ToJsonSerializerSettings();
                object jsonObject = JsonConvert.DeserializeObject(input, options);

                options.Formatting = Formatting.Indented;
                return JsonConvert.SerializeObject(jsonObject, options);
            }
            catch (Exception)
            {
                return input;
            }
        }

        /// <summary>
        /// Formats the input XML with whitespace.
        /// </summary>
        /// <param name="input">The input XML string</param>
        /// <returns>The formatted XML output.</returns>
        public static string FormatXml(string input)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            try
            {
                XDocument document = XDocument.Parse(input);

                return String.Format(CultureInfo.InvariantCulture,
                                     "{0}{1}{2}",
                                     document.Declaration,
                                     Environment.NewLine,
                                     document.ToString(SaveOptions.OmitDuplicateNamespaces));
            }
            catch (Exception)
            {
                return input;
            }
        }
    }
}
