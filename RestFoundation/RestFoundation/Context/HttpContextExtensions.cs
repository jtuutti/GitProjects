using System;
using System.Linq;
using System.Net;
using System.Web;

namespace RestFoundation.Context
{
    internal static class HttpContextExtensions
    {
        private const string HttpMethodOverrideHeader = "X-HTTP-Method-Override";

        public static HttpMethod GetOverriddenHttpMethod(this HttpContextBase context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            string httpMethodString;

            if (String.Equals("POST", context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                httpMethodString = context.Request.Headers.Get(HttpMethodOverrideHeader);

                if (String.IsNullOrEmpty(httpMethodString) && context.Request.AcceptTypes != null &&
                    context.Request.AcceptTypes.Contains("application/x-www-form-urlencoded", StringComparer.OrdinalIgnoreCase))
                {
                    httpMethodString = context.Request.Form.Get(HttpMethodOverrideHeader);
                }

                if (String.IsNullOrEmpty(httpMethodString))
                {
                    httpMethodString = context.Request.HttpMethod;
                }
            }
            else
            {
                httpMethodString = context.Request.HttpMethod;
            }

            HttpMethod httpMethod;

            if (!Enum.TryParse(httpMethodString, true, out httpMethod))
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed, "HTTP method is not allowed");
            }

            return httpMethod;
        }
    }
}
