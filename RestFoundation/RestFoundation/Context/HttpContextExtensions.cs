// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Linq;
using System.Net;
using System.Web;
using RestFoundation.Runtime;

namespace RestFoundation.Context
{
    internal static class HttpContextExtensions
    {
        private const string HttpMethodOverrideHeader = "X-HTTP-Method-Override";
        private const string FormDataMediaType = "application/x-www-form-urlencoded";

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
                    context.Request.AcceptTypes.Contains(FormDataMediaType, StringComparer.OrdinalIgnoreCase))
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
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed, Resources.Global.DisallowedHttpMethod);
            }

            return httpMethod;
        }
    }
}
