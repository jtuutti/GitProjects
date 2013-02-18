using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace RestFoundation.Runtime
{
    internal static class AllowHeaderParser
    {
        public static IReadOnlyList<HttpMethod> Parse(NameValueCollection headers)
        {
            if (headers == null || headers.Count == 0)
            {
                return new ReadOnlyCollection<HttpMethod>(new HttpMethod[0]);
            }

            string allowHeaderValue = headers.Get("Allow");

            if (String.IsNullOrWhiteSpace(allowHeaderValue))
            {
                return new ReadOnlyCollection<HttpMethod>(new HttpMethod[0]);
            }

            return ParseHeaderValue(allowHeaderValue);
        }

        private static IReadOnlyList<HttpMethod> ParseHeaderValue(string headerValue)
        {
            var allowedMethods = new List<HttpMethod>();
            string[] allowedHeaderValues = headerValue.Split(',');

            for (int i = 0; i < allowedHeaderValues.Length; i++)
            {
                string allowedHeaderValue = allowedHeaderValues[i];
                HttpMethod allowedMethod;

                if (allowedHeaderValue != null && Enum.TryParse(allowedHeaderValue.Trim(), true, out allowedMethod) && allowedMethod != HttpMethod.Options)
                {
                    allowedMethods.Add(allowedMethod);
                }
            }

            return new ReadOnlyCollection<HttpMethod>(allowedMethods);
        }
    }
}
