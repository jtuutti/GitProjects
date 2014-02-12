// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Text.RegularExpressions;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Converts a pascal-case descriptor into a sentence.
    /// </summary>
    public static class PascalCaseToSentenceConverter
    {
        private static readonly Regex firstLetterRegex = new Regex(@"([^^])([A-Z])", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Separates pascal case descriptor into separate words.
        /// Ex: FileNotFound -> File Not Found
        /// </summary>
        /// <param name="input">The input <see cref="string"/>.</param>
        /// <returns>A <see cref="string"/> containing the converted input.</returns>
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
