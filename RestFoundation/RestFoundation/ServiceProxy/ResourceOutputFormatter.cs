using System;
using System.Globalization;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RestFoundation.ServiceProxy
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
                if (input.TrimStart().StartsWith("[", StringComparison.Ordinal))
                {
                    return JArray.Parse(input).ToString(Formatting.Indented);
                }

                return JObject.Parse(input).ToString(Formatting.Indented);
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
