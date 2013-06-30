// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents a resource output formatter that adds whitespace to improve JSON/XML readability.
    /// </summary>
    public static class ResourceOutputFormatter
    {
        private static readonly Regex JsonPRegex = new Regex(@"^(jsonpCallback\()(.+)(\);?)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
            
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
        /// Formats the input JSONP with whitespace.
        /// </summary>
        /// <param name="input">The input JSONP string</param>
        /// <returns>The formatted JSONP output.</returns>
        public static string FormatJsonP(string input)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            var match = JsonPRegex.Match(input);

            if (match.Groups.Count != 4)
            {
                return input;
            }

            try
            {
                var options = Rest.Configuration.Options.JsonSettings.ToJsonSerializerSettings();
                object jsonObject = JsonConvert.DeserializeObject(match.Groups[2].Value, options);

                options.Formatting = Formatting.Indented;
                return String.Concat(match.Groups[1].Value, JsonConvert.SerializeObject(jsonObject, options), match.Groups[3].Value);
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
