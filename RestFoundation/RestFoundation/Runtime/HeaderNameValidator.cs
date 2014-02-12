// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System.Text.RegularExpressions;

namespace RestFoundation.Runtime
{
    internal static class HeaderNameValidator
    {
        public static bool IsValid(string headerName)
        {
            return headerName != null && !Regex.IsMatch(headerName, @"\s");
        }
    }
}
