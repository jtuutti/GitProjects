// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using RestFoundation.Formatters;
using RestFoundation.Results;
using RestFoundation.Runtime;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Validation;

namespace RestFoundation
{
    /// <summary>
    /// Represents an HTTP module required by the REST foundation.
    /// </summary>
    public sealed class RestHttpModule : IHttpModule
    {
        private const string DangerousRequestMessage = "A potentially dangerous Request.Path value was detected from the client";
        private const string RequestTimeoutMessage = "Request timed out";

        internal static bool IsInitialized { get; set; }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.Web.HttpApplication"/> that provides access to the methods, properties, and events common to all
        /// application objects within an ASP.NET application.
        /// </param>
        public void Init(HttpApplication context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            IsInitialized = true;

            context.PreRequestHandlerExecute += (sender, args) => OnPreRequestHandlerExecute(sender);
            context.PreSendRequestHeaders += (sender, args) => OnPreSendRequestHeaders(sender);
            context.EndRequest += (sender, args) => OnEndRequest(sender);
            context.Error += (sender, args) => OnError(sender);
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose()
        {
        }

        private static void OnPreRequestHandlerExecute(object sender)
        {
            var application = sender as HttpApplication;

            if (application == null)
            {
                return;
            }

            if (IsRestHandler(application))
            {
                Rest.Configuration.Options.BeginRequestAction(Rest.Configuration.ServiceLocator.GetService<IServiceContext>());
            }
            else
            {
                TryIngestPageDependencies(application);
            }
        }

        private static void OnPreSendRequestHeaders(object sender)
        {
            var application = sender as HttpApplication;

            if (application == null)
            {
                return;
            }

            RemoveServerHeaders(application);
            SetResponseHeaders(application);
        }

        private static void OnEndRequest(object sender)
        {
            var application = sender as HttpApplication;

            if (application == null || !IsRestHandler(application))
            {
                return;
            }

            try
            {
                Rest.Configuration.Options.EndRequestAction(Rest.Configuration.ServiceLocator.GetService<IServiceContext>());
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
            }
            finally
            {
                LogServiceCallEnd();
            }
        }

        private static void OnError(object sender)
        {
            var application = sender as HttpApplication;

            if (application == null)
            {
                return;
            }

            Exception exception = TryGetException(application);

            if (exception != null && IsRestHandler(application))
            {
                Rest.Configuration.Options.ExceptionAction(Rest.Configuration.ServiceLocator.GetService<IServiceContext>(), exception);
            }
        }

        private static bool IsRestHandler(HttpApplication application)
        {
            IHttpHandler handler = application.Context.CurrentHandler;

            return handler is IRestHandler && !(handler is RootRouteHandler);
        }

        private static void TryIngestPageDependencies(HttpApplication application)
        {
            var handler = application.Context.CurrentHandler as Page;

            if (handler == null)
            {
                return;
            }

            WebFormsInjectionHelper.InjectControlDependencies(handler);

            handler.PreInit += (s, e) => WebFormsInjectionHelper.InitializeChildControls(handler);
        }

        private static void SetResponseHeaders(HttpApplication application)
        {
            IDictionary<string, string> responseHeaders = Rest.Configuration.Options.ResponseHeaders;

            if (responseHeaders == null || responseHeaders.Count == 0)
            {
                return;
            }

            foreach (var header in responseHeaders)
            {
                if (!HeaderNameValidator.IsValid(header.Key))
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError, RestResources.EmptyHttpHeader);
                }

                application.Response.AppendHeader(header.Key, header.Value);
            }
        }

        private static void RemoveServerHeaders(HttpApplication application)
        {
            if (!HttpRuntime.UsingIntegratedPipeline)
            {
                return;
            }

            application.Response.Headers.Remove("Server");
            application.Response.Headers.Remove("X-AspNet-Version");
            application.Response.Headers.Remove("X-Powered-By");
        }

        private static Exception TryGetException(HttpApplication application)
        {
            Exception exception = application.Server.GetLastError();

            if (exception is HttpUnhandledException && exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            var responseException = exception as HttpResponseException;

            if (responseException != null)
            {
                SetResponseStatus(application, responseException.StatusCode, responseException.StatusDescription);
                return null;
            }

            var faultException = exception as HttpResourceFaultException;

            if (faultException != null)
            {
                SetFaultResponse(application, faultException);
                return null;
            }

            TryClearCompressionFilter(application);

            if (exception != null)
            {
                LogException(exception);
            }

            var validationException = exception as HttpRequestValidationException;

            if (validationException != null)
            {
                SetResponseStatus(application, HttpStatusCode.Forbidden, RestResources.ValidationRequestFailed);
                return null;
            }

            var httpException = exception as HttpException;

            if (httpException != null)
            {
                if (httpException.Message.Contains(DangerousRequestMessage))
                {
                    SetResponseStatus(application, HttpStatusCode.Forbidden, RestResources.ValidationRequestFailed);
                    return null;
                }

                if (httpException.Message.Contains(RequestTimeoutMessage))
                {
                    SetResponseStatus(application, HttpStatusCode.ServiceUnavailable, RestResources.ServiceTimedOut);
                    return null;
                }
            }

            return exception;
        }

        private static void SetResponseStatus(HttpApplication application, HttpStatusCode statusCode, string statusDescription)
        {
            application.Response.Clear();
            application.Response.StatusCode = (int) statusCode;
            application.Response.StatusDescription = HttpUtility.HtmlEncode(statusDescription);
            application.Server.ClearError();
            application.CompleteRequest();
        }

        private static void SetFaultResponse(HttpApplication application, HttpResourceFaultException faultException)
        {
            application.Server.ClearError();
            application.Response.Clear();
            application.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            application.Response.StatusDescription = RestResources.ResourceValidationFailed;

            if ("POST".Equals(application.Request.HttpMethod, StringComparison.OrdinalIgnoreCase) || "PUT".Equals(application.Request.HttpMethod, StringComparison.OrdinalIgnoreCase) ||
                "PATCH".Equals(application.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                application.Response.TrySkipIisCustomErrors = true;

                OutputResourceValidationFaults(faultException);
            }

            application.CompleteRequest();
        }

        private static void TryClearCompressionFilter(HttpApplication application)
        {
            try
            {
                if (application.Response.Filter != null && application.Response.Filter.GetType().Namespace != typeof(HttpResponse).Namespace)
                {
                    application.Response.Filter = null;
                }
            }
            catch (Exception)
            {
            }
        }

        private static void OutputResourceValidationFaults(HttpResourceFaultException faultException)
        {
            var context = Rest.Configuration.ServiceLocator.GetService<IServiceContext>();
            var faultCollection = GenerateFaultCollection(context, faultException);

            if (faultCollection.General.Length == 0 && faultCollection.Resource.Length == 0)
            {
                return;
            }

            var contentNegotiator = Rest.Configuration.ServiceLocator.GetService<IContentNegotiator>();
            var formatter = MediaTypeFormatterRegistry.GetFormatter(contentNegotiator.GetPreferredMediaType(context.Request));

            if (formatter != null)
            {
                ExecuteFaultResult(formatter, context, faultCollection);
            }
        }

        private static FaultCollection GenerateFaultCollection(IServiceContext context, HttpResourceFaultException faultException)
        {
            var validationErrors = new List<ValidationError>();
            validationErrors.AddRange(faultException.FaultMessages.Where(m => !String.IsNullOrEmpty(m)).Select(m => new ValidationError(m)));
            validationErrors.AddRange(context.Request.ResourceState ?? new ResourceState(new ValidationError[0]));

            if (validationErrors.Count == 0)
            {
                return new FaultCollection();
            }

            return new FaultCollection
            {
                General = validationErrors.Where(e => String.IsNullOrEmpty(e.PropertyName)).Select(e => new Fault
                {
                    PropertyName = e.PropertyName,
                    Message = e.Message
                }).ToArray(),
                Resource = validationErrors.Where(e => !String.IsNullOrEmpty(e.PropertyName)).Select(e => new Fault
                {
                    PropertyName = e.PropertyName,
                    Message = e.Message
                }).ToArray()
            };
        }

        private static void ExecuteFaultResult(IMediaTypeFormatter formatter, IServiceContext context, FaultCollection faultCollection)
        {
            IResult result = formatter.FormatResponse(context, faultCollection.GetType(), faultCollection);

            if (result != null)
            {
                result.Execute(context);
            }
        }

        private static void LogServiceCallEnd()
        {
            if (LogUtility.CanLog)
            {
                LogUtility.Writer.WriteInfo(String.Empty)
                            .WriteInfo("--- SERVICE CALL ENDED ---")
                            .WriteInfo(String.Empty)
                            .Flush();
            }
        }

        private static void LogException(Exception exception)
        {
            if (!LogUtility.CanLog)
            {
                return;
            }

            LogUtility.Writer.WriteInfo("EXCEPTION OCCURRED:")
                      .WriteInfo(exception.ToString())
                      .WriteInfo(String.Empty);

            LogUtility.Writer.WriteError("EXCEPTION OCCURRED:")
                      .WriteError(exception.ToString())
                      .WriteError(String.Empty);
        }
    }
}
