using System;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RestFoundation.ServiceProxy
{
    public static class ResourceOutputFormatter
    {
        public static string FormatXml(string input)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            try
            {
                XDocument document = XDocument.Parse(input);

                return String.Format("{0}{1}{2}", document.Declaration, Environment.NewLine, document.ToString(SaveOptions.OmitDuplicateNamespaces));
            }
            catch (Exception)
            {
                return input;
            }
        }

        public static string FormatJson(string input)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            try
            {
                if (input.TrimStart().StartsWith("["))
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
    }
}
