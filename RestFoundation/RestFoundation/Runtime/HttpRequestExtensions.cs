using System;
using RestFoundation.Collections.Specialized;

namespace RestFoundation.Runtime
{
    internal static class HttpRequestExtensions
    {
        private const string AcceptOverrideQueryValue = "X-Accept-Override";

        public static string GetPreferredAcceptType(this IHttpRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");

            string acceptValue = request.QueryString.TryGet(AcceptOverrideQueryValue);

            if (String.IsNullOrEmpty(acceptValue))
            {
                acceptValue = request.Headers.AcceptType;
            }

            if (String.IsNullOrEmpty(acceptValue))
            {
                acceptValue = request.Headers.ContentType;
            }

            return new AcceptValueCollection(acceptValue).GetPreferredName();
        }
    }
}
