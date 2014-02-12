// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;

namespace RestFoundation.Runtime
{
    internal static class StatusDescriptionFormatter
    {
        public static string Format(string description)
        {
            if (description == null)
            {
                return String.Empty;
            }

            var lineBreakCharacters = new[] { '\r', '\n' };

            if (description.IndexOfAny(lineBreakCharacters) >= 0)
            {
                description = description.Split(lineBreakCharacters, StringSplitOptions.None)[0];
            }

            const int MaxStatusDescriptionLength = 512;

            if (description.Length > MaxStatusDescriptionLength)
            {
                description = description.Substring(0, MaxStatusDescriptionLength);
            }

            return description.Trim();
        }
    }
}
