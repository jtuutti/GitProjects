using System;
using System.Text.RegularExpressions;

namespace RestFoundation.Runtime
{
    internal static class UrlTemplateStandardizer
    {
        private static readonly Regex routeParameterRegex = new Regex(@"\{([^\}]+?)\}", RegexOptions.Compiled);

        public static string Standardize(string urlTemplate)
        {
            if (String.IsNullOrEmpty(urlTemplate))
            {
                return urlTemplate;
            }

            int parameterNumber = 0;
            return routeParameterRegex.Replace(urlTemplate, match => String.Concat("{p", parameterNumber++ + "}"));
        }
    }
}
