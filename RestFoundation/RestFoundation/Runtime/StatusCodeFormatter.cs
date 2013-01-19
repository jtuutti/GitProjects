// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Text.RegularExpressions;

namespace RestFoundation.Runtime
{
    internal static class PascalCaseToSentenceConverter
    {
        private static readonly Regex firstLetterRegex = new Regex(@"([^^])([A-Z])", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static string Convert(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return input;
            }

            return firstLetterRegex.Replace(input, "$1 $2");
        }
    }
}
