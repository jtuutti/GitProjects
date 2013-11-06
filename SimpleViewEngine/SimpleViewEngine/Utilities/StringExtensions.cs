using System;

namespace SimpleViewEngine.Utilities
{
    internal static class StringExtensions
    {
        public static string TrimLine(this string value)
        {
            return !String.IsNullOrEmpty(value) ? value.Trim(' ', '\n', '\r', '\t') : value;
        }
    }
}
