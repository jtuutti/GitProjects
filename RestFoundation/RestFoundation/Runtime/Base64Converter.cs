using System;
using System.Text;

namespace RestFoundation.Runtime
{
    internal static class Base64Converter
    {
        public static string Decode(string base64EncodedString)
        {
            if (base64EncodedString == null)
            {
                return null;
            }

            byte[] decodedStringInBytes = Convert.FromBase64String(base64EncodedString);
            return Encoding.UTF8.GetString(decodedStringInBytes);
        }

        public static string Encode(string stringToBase64Encode)
        {
            if (stringToBase64Encode == null)
            {
                return null;
            }

            byte[] encodedStringInBytes = Encoding.UTF8.GetBytes(stringToBase64Encode);
            return Convert.ToBase64String(encodedStringInBytes);
        }
    }
}
