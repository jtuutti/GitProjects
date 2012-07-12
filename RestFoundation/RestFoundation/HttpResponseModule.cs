using System;
using System.Net;
using System.Web;
using System.Web.UI;
using RestFoundation.Runtime;

namespace RestFoundation
{
    public sealed class HttpResponseModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            if (!HttpRuntime.UsingIntegratedPipeline)
            {
                throw new HttpException(500, "Rest Foundation services can only run under the IIS 7+ integrated pipeline mode");
            }

            context.PreRequestHandlerExecute += (sender, args) => IngestPageDependencies(context);
            context.PreSendRequestHeaders += (sender, args) => RemoveServerHeaders(context);
            context.Error += (sender, args) => CompleteRequestOnError(context);
            context.EndRequest += (sender, args) => SetResponseHeaders(context);
        }

        public void Dispose()
        {
        }

        private static void IngestPageDependencies(HttpApplication context)
        {
            var handler = context.Context.CurrentHandler as Page;

            if (handler != null)
            {
                Rest.Active.Activator.BuildUp(handler);
            }
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

            if (exception is HttpUnhandledException && exception.InnerException != null)
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
                context.Response.StatusDescription = HttpUtility.HtmlEncode(statusDescription);
                context.Server.ClearError();
                context.CompleteRequest();
            }
            catch (Exception)
            {
            }
        }

        private static void SetResponseHeaders(HttpApplication context)
        {
            if (Rest.Active.ResponseHeaders == null || Rest.Active.ResponseHeaders.Count == 0)
            {
                return;
            }

            foreach (var header in Rest.Active.ResponseHeaders)
            {
                if (!HeaderNameValidator.IsValid(header.Key))
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError, "HTTP headers cannot be empty or have whitespace in the name");
                }

                context.Response.Headers.Add(header.Key, header.Value);
            }
        }
    }
}
