// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Routing;
using System.Web.UI;
using RestFoundation.Collections.Specialized;
using RestFoundation.Configuration;
using RestFoundation.Resources;
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
        private const string AcceptPatchHeaderName = "Accept-Patch";
        private const string CatchAllRoute = "catch-all-route";
        private const string DangerousRequestMessage = "A potentially dangerous Request.Path value was detected from the client";
        private const string HeaderValueSeparator = ", ";
        private const string LocationHeaderName = "Location";
        private const string Options = "OPTIONS";
        private const string RequestTimeoutMessage = "Request timed out";

        private static readonly object syncRoot = new Object();
        private static volatile bool catchAllRouteInitialized;

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
            var application = sender as HttpApplication;

            if (application == null || application.Context == null)
            {
                return;
            }

            var rewriter = Rest.Configuration.ServiceLocator.GetService<IUrlRewriter>();

            if (rewriter != null && !(rewriter is NullUrlRewriter) && RewriteUrl(rewriter, application))
            {
                return;
            }

            if (catchAllRouteInitialized)
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
                TryInjectPageDependencies(application);
            }
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

        private static bool RewriteUrl(IUrlRewriter rewriter, HttpApplication application)
        {
            string rewrittenUrl = rewriter.RewriteUrl(application.Context.Request.RawUrl, application.Request.Headers);

            if (String.IsNullOrWhiteSpace(rewrittenUrl))
            {
                return false;
            }

            switch (rewriter.RewriteType)
            {
                case UrlRewriteType.TemporaryRedirect:
                    application.Context.Response.AppendHeader(LocationHeaderName, rewrittenUrl);
                    application.Context.Response.StatusCode = (int) RedirectType.TemporaryRedirect;
                    application.Context.Response.StatusDescription = PascalCaseToSentenceConverter.Convert(RedirectType.TemporaryRedirect.ToString());
                    application.Context.Response.End();
                    return true;
                case UrlRewriteType.PermanentRedirect:
                    application.Context.Response.AppendHeader(LocationHeaderName, rewrittenUrl);
                    application.Context.Response.StatusCode = (int) RedirectType.PermanentRedirect;
                    application.Context.Response.StatusDescription = PascalCaseToSentenceConverter.Convert(RedirectType.PermanentRedirect.ToString());
                    application.Context.Response.End();
                    return true;
                default:
                    application.Context.RewritePath(rewrittenUrl);
                    return false;                   
            }
        }

        private static bool IsRestHandler(HttpApplication application)
        {
            IHttpHandler handler = application.Context.CurrentHandler;

            return handler is IRestServiceHandler && !(handler is RootRouteHandler);
        }

        private static void TryInjectPageDependencies(HttpApplication application)
        {
            var handler = application.Context.CurrentHandler as Page;

            if (handler == null)
            {
                return;
            }

            WebFormsInjectionHelper.InjectControlDependencies(handler);

            handler.PreInit += (s, e) => WebFormsInjectionHelper.InitializeChildControls(handler);
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
                OutputStatus(application, HttpStatusCode.Forbidden, Global.ValidationRequestFailed);
                return null;
            }

            var httpException = exception as HttpException;

            if (httpException != null)
            {
                if (httpException.Message.Contains(DangerousRequestMessage))
                {
                    OutputStatus(application, HttpStatusCode.Forbidden, Global.ValidationRequestFailed);
                    return null;
                }

                if (httpException.Message.Contains(RequestTimeoutMessage))
                {
                    OutputStatus(application, HttpStatusCode.ServiceUnavailable, Global.ServiceTimedOut);
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

            object constraintFailedFlag = application.Context.Items[ServiceCallConstants.RouteMethodConstraintFailed];

            if (constraintFailedFlag == null || !Convert.ToBoolean(constraintFailedFlag, CultureInfo.InvariantCulture))
            {
                return;
            }

            Fault statusFault = faults.General.FirstOrDefault(f => f.Message == Global.NotFound);

            if (statusFault != null)
            {
                statusFault.Message = Global.DisallowedHttpMethod;
            }

            statusCode = HttpStatusCode.MethodNotAllowed;
            statusDescription = Global.DisallowedHttpMethod;
        }

        private static IEnumerable<HttpMethod> SetAllowHeaderForNotSupportedHttpMethod(HttpApplication application)
        {
            var allowedHttpMethods = application.Context.Items[ServiceCallConstants.AllowedHttpMethods] as HttpMethodCollection;
            string allowedMethodString = allowedHttpMethods != null ? allowedHttpMethods.ToString() : "GET, HEAD";

            application.Context.Response.AppendHeader("Allow", String.Concat(allowedMethodString, HeaderValueSeparator, Options));

            return allowedHttpMethods != null ? allowedHttpMethods.ToList() : new ReadOnlyCollection<HttpMethod>(new[] { HttpMethod.Get, HttpMethod.Head });
        }

        private static void SetAcceptPatchHeader(IServiceContext context)
        {
            IReadOnlyCollection<string> mediaTypes = MediaTypeFormatterRegistry.GetPrioritizedMediaTypes();

            if (mediaTypes != null && mediaTypes.Count > 0)
            {
                context.Response.AppendHeader(AcceptPatchHeaderName, String.Join(HeaderValueSeparator, mediaTypes));
            }
        }

        private static void GenerateFaultBody(IServiceContext context, IServiceContextHandler handler, FaultCollection faults)
        {
            if (faults.General.Length == 0 && faults.Resource.Length == 0)
            {
                return;
            }

            var resultWrapper = Rest.Configuration.ServiceLocator.GetService<IResultWrapper>();

            IResult result;

            try
            {
                result = resultWrapper.Wrap(handler, faults, faults.GetType());
            }
            catch (HttpResponseException ex)
            {
                context.Response.SetStatus(ex.StatusCode, ex.StatusDescription);
                return;
            }

            if (result != null)
            {
                result.Execute(context);
            }
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
            application.Response.Clear();

            HandleNotSupportedHttpMethod(application, faults, ref statusCode, ref statusDescription);
            context.Response.SetStatus(statusCode, statusDescription);

            if (statusCode == HttpStatusCode.MethodNotAllowed)
            {
                IEnumerable<HttpMethod> methods = SetAllowHeaderForNotSupportedHttpMethod(application);

                if (methods.Contains(HttpMethod.Patch))
                {
                    SetAcceptPatchHeader(context);
                }

                if (String.Equals(Options, application.Context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.SetStatus(HttpStatusCode.OK, Global.OK);
                    return;
                }
            }

            var contextNegotiator = Rest.Configuration.ServiceLocator.GetService<IContentNegotiator>();

            if (!contextNegotiator.IsBrowserRequest(context.Request))
            {
                context.Response.TrySkipIisCustomErrors = true;

                // Do not try to resolve the response content-type because there is a problem with the Accept header
                if (statusCode != HttpStatusCode.NotAcceptable)
                {
                    GenerateFaultBody(context, handler, faults);
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

            OutputFaults(application, HttpStatusCode.InternalServerError, Global.InternalServerError, faults);
        }

        private static void OutputValidationFaults(HttpApplication application, HttpResourceFaultException faultException)
        {
            var context = Rest.Configuration.ServiceLocator.GetService<IServiceContext>();
            FaultCollection faults = GenerateFaultCollection(context, faultException);

            if (faults.General.Length == 0 && faults.Resource.Length == 0)
            {
                return;
            }

            OutputFaults(application, HttpStatusCode.BadRequest, Global.ResourceValidationFailed, faults);
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
