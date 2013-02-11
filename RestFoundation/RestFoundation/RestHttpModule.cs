// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Routing;
using System.Web.UI;
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
        private const int MinErrorStatusCode = 400;
        private const int UnauthorizedStatusCode = 401;
        private const string DangerousRequestMessage = "A potentially dangerous Request.Path value was detected from the client";
        private const string RequestTimeoutMessage = "Request timed out";
        private const string CatchAllRoute = "catch-all-route";

        private static readonly object syncRoot = new object();
        private static bool catchAllRouteInitialized;

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

            context.BeginRequest += (sender, args) => OnBeginRequest(sender);
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

        private static void OnBeginRequest(object sender)
        {
            if (catchAllRouteInitialized)
            {
                return;
            }

            var application = sender as HttpApplication;

            if (application == null || application.Context == null)
            {
                return;
            }

            lock (syncRoot)
            {
                if (catchAllRouteInitialized)
                {
                    return;
                }

                var context = new HttpContextWrapper(application.Context);

                if (RouteTable.Routes.Any(x =>
                {
                    RouteData routeData = x.GetRouteData(context);
                    return routeData != null && Convert.ToBoolean(routeData.DataTokens[CatchAllRoute], CultureInfo.InvariantCulture);
                }))
                {
                    return;
                }

                RouteTable.Routes.Add(new Route("{*url}", null, null, new RouteValueDictionary { { CatchAllRoute, true } }, new NotFoundHandler()));

                catchAllRouteInitialized = true;
            }
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
                OutputInternalException(application, ex);

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

            if (exception == null || !IsRestHandler(application))
            {
                return;
            }

            Rest.Configuration.Options.ExceptionAction(Rest.Configuration.ServiceLocator.GetService<IServiceContext>(), exception);

            OutputInternalException(application, exception);
        }

        private static bool IsRestHandler(HttpApplication application)
        {
            IHttpHandler handler = application.Context.CurrentHandler;

            return handler is IRestServiceHandler && !(handler is RootRouteHandler);
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
                    OutputStatus(application, HttpStatusCode.InternalServerError, RestResources.EmptyHttpHeader);
                    return;
                }

                application.Response.AppendHeader(header.Key, header.Value);
            }
        }

        private static void RemoveServerHeaders(HttpApplication application)
        {
            if (!HttpRuntime.UsingIntegratedPipeline || Rest.Configuration.Options.RetainWebServerHeaders)
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
                OutputStatus(application, responseException.StatusCode, responseException.StatusDescription);
                return null;
            }

            var faultException = exception as HttpResourceFaultException;

            if (faultException != null)
            {
                OutputValidationFaults(application, faultException);
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
                OutputStatus(application, HttpStatusCode.Forbidden, RestResources.ValidationRequestFailed);
                return null;
            }

            var httpException = exception as HttpException;

            if (httpException != null)
            {
                if (httpException.Message.Contains(DangerousRequestMessage))
                {
                    OutputStatus(application, HttpStatusCode.Forbidden, RestResources.ValidationRequestFailed);
                    return null;
                }

                if (httpException.Message.Contains(RequestTimeoutMessage))
                {
                    OutputStatus(application, HttpStatusCode.ServiceUnavailable, RestResources.ServiceTimedOut);
                    return null;
                }
            }

            return exception;
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

        private static void HandleNotSupportedHttpMethod(HttpApplication application, FaultCollection faults, ref HttpStatusCode statusCode, ref string statusDescription)
        {
            if (statusCode != HttpStatusCode.NotFound)
            {
                return;
            }

            object constraintFailedFlag = application.Context.Items[RouteConstants.RouteMethodConstraintFailed];

            if (constraintFailedFlag == null || !Convert.ToBoolean(constraintFailedFlag, CultureInfo.InvariantCulture))
            {
                return;
            }

            Fault statusFault = faults.General.FirstOrDefault(f => f.Message == RestResources.NotFound);

            if (statusFault != null)
            {
                statusFault.Message = RestResources.DisallowedHttpMethod;
            }

            statusCode = HttpStatusCode.MethodNotAllowed;
            statusDescription = RestResources.DisallowedHttpMethod;
        }

        private static void OutputFaults(HttpApplication application, HttpStatusCode statusCode, string statusDescription, FaultCollection faults)
        {
            if (faults == null)
            {
                throw new ArgumentNullException("faults");
            }

            var context = Rest.Configuration.ServiceLocator.GetService<IServiceContext>();
            var handler = application.Context.CurrentHandler as IServiceContextHandler ?? new ServiceContextHandler(context);

            application.Server.ClearError();
            context.Response.Output.Clear();

            HandleNotSupportedHttpMethod(application, faults, ref statusCode, ref statusDescription);
            context.Response.SetStatus(statusCode, statusDescription);

            var contextNegotiator = Rest.Configuration.ServiceLocator.GetService<IContentNegotiator>();

            if (!contextNegotiator.IsBrowserRequest(context.Request))
            {
                context.Response.TrySkipIisCustomErrors = true;

                if (faults.General.Length > 0 || faults.Resource.Length > 0)
                {
                    var resultFactory = Rest.Configuration.ServiceLocator.GetService<IResultFactory>();

                    IResult result = resultFactory.Create(faults, faults.GetType(), handler);

                    if (result != null)
                    {
                        result.Execute(context);
                    }
                }
            }

            application.CompleteRequest();
        }

        private static void OutputStatus(HttpApplication application, HttpStatusCode statusCode, string statusDescription)
        {
            var faults = new FaultCollection();
            var numericStatusCode = (int) statusCode;

            if (numericStatusCode >= MinErrorStatusCode && numericStatusCode != UnauthorizedStatusCode)
            {
                faults.General = new[]
                {
                    new Fault
                    {
                        Message = statusDescription
                    }
                };
            }

            OutputFaults(application, statusCode, statusDescription, faults);
        }

        private static void OutputInternalException(HttpApplication application, Exception exception)
        {
            var context = Rest.Configuration.ServiceLocator.GetService<IServiceContext>();

            if (!context.Request.IsLocal)
            {
                return;
            }

            var contextNegotiator = Rest.Configuration.ServiceLocator.GetService<IContentNegotiator>();

            if (contextNegotiator.IsBrowserRequest(context.Request))
            {
                return;
            }

            var unwrappedException = ExceptionUnwrapper.Unwrap(exception);

            var faults = new FaultCollection
            {
                General = new[]
                {
                    new Fault
                    {
                        Message = unwrappedException.Message,
                        Detail = GetExceptionDetail(application, unwrappedException)
                    }
                }
            };

            OutputFaults(application, HttpStatusCode.InternalServerError, RestResources.InternalServerError, faults);
        }

        private static void OutputValidationFaults(HttpApplication application, HttpResourceFaultException faultException)
        {
            var context = Rest.Configuration.ServiceLocator.GetService<IServiceContext>();
            FaultCollection faults = GenerateFaultCollection(context, faultException);

            if (faults.General.Length == 0 && faults.Resource.Length == 0)
            {
                return;
            }

            OutputFaults(application, HttpStatusCode.BadRequest, RestResources.ResourceValidationFailed, faults);
        }

        private static string GetExceptionDetail(HttpApplication application, Exception exception)
        {
            FaultDetail detail = Rest.Configuration.Options.FaultDetail;

            if (detail == FaultDetail.MessageOnly || (detail == FaultDetail.DetailedInDebugMode && !application.Context.IsDebuggingEnabled))
            {
                return null;
            }

            return exception.ToString();
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
