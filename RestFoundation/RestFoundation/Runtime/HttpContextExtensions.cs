using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace RestFoundation.Runtime
{
    internal static class HttpContextExtensions
    {
        private const string AllowHeader = "Allow";
        private const string HttpMethodOverrideHeader = "X-HTTP-Method-Override";

        public static void AppendAllowHeader(this HttpContext context, IEnumerable<HttpMethod> allowedHttpMethods)
        {
                context.Response.AppendHeader(AllowHeader, String.Join(", ", allowedHttpMethods.Select(m => m.ToString().ToUpperInvariant()).OrderBy(m => m)));
        }

        public static HttpMethod GetOverriddenHttpMethod(this HttpContext context)
        {
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

        public static void SetServiceMethodResponseStatus(this HttpContext context, Type methodReturnType)
        {
            if (methodReturnType == typeof(void))
            {
                context.Response.StatusCode = 204;
                context.Response.StatusDescription = String.Empty;
            }
            else
            {
                context.Response.StatusCode = 200;
                context.Response.StatusDescription = "OK";
            }
        }
    }
}
