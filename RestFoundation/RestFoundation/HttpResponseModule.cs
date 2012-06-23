using System;
using System.Net;
using System.Web;

namespace RestFoundation
{
    public sealed class HttpResponseModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += (sender, args) => RemoveServerHeaders(context);
            context.Error += (sender, args) => CompleteRequestOnError(context);
        }

        public void Dispose()
        {
        }

        private static void RemoveServerHeaders(HttpApplication context)
        {
            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-AspNet-Version");
            context.Response.Headers.Remove("X-Powered-By");
        }

        private static void CompleteRequestOnError(HttpApplication context)
        {
            Exception exception = context.Server.GetLastError();

            if (exception is HttpUnhandledException && exception.InnerException == null)
            {
                exception = exception.InnerException;
            }

            var responseException = exception as HttpResponseException;

            if (responseException != null)
            {
                SetResponseStatus(context, responseException.StatusCode, responseException.StatusDescription);
                return;
            }

            var validationException = exception as HttpRequestValidationException;

            if (validationException != null)
            {
                SetResponseStatus(context, HttpStatusCode.Forbidden, "A potentially dangerous value was found in the HTTP request");
                return;
            }

            var httpException = exception as HttpException;

            if (httpException != null && httpException.Message.Contains("A potentially dangerous Request.Path value was detected from the client"))
            {
                SetResponseStatus(context, HttpStatusCode.Forbidden, "A potentially dangerous value was found in the HTTP request");
            }
        }

        private static void SetResponseStatus(HttpApplication context, HttpStatusCode statusCode, string statusDescription)
        {
            try
            {
                context.Response.Clear();
                context.Response.StatusCode = (int) statusCode;
                context.Response.StatusDescription = statusDescription;
                context.Server.ClearError();
                context.CompleteRequest();
            }
            catch (Exception)
            {
            }
        }
    }
}
