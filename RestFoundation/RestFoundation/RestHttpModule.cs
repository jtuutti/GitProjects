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

        private HttpApplication m_context;
        private bool m_integratedPipeline;

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

            m_context = context;
            m_integratedPipeline = HttpRuntime.UsingIntegratedPipeline;

            context.Error += (sender, args) => CompleteRequestOnError();
            context.PreRequestHandlerExecute += (sender, args) => IngestPageDependencies();
            context.PreSendRequestHeaders += (sender, args) =>
            {
                RemoveServerHeaders();
                SetResponseHeaders();
            };
            context.EndRequest += (sender, args) =>
            {
                if (LogUtility.CanLog)
                {
                    LogUtility.Writer.WriteInfo(String.Empty)
                                     .WriteInfo("--- SERVICE CALL ENDED ---")
                                     .WriteInfo(String.Empty)
                                     .Flush();
                }
            };
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose()
        {
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

        private void SetResponseStatus(HttpStatusCode statusCode, string statusDescription)
        {
            m_context.Response.Clear();
            m_context.Response.StatusCode = (int) statusCode;
            m_context.Response.StatusDescription = HttpUtility.HtmlEncode(statusDescription);
            m_context.Server.ClearError();
            m_context.CompleteRequest();
        }

        private void CompleteRequestOnError()
        {
            Exception exception = m_context.Server.GetLastError();

            if (exception is HttpUnhandledException && exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            var responseException = exception as HttpResponseException;

            if (responseException != null)
            {
                SetResponseStatus(responseException.StatusCode, responseException.StatusDescription);
                return;
            }

            var faultException = exception as HttpResourceFaultException;

            if (faultException != null)
            {
                SetFaultResponse(faultException);
                return;
            }

            TryClearCompressionFilter();

            if (exception != null && LogUtility.CanLog)
            {
                LogUtility.Writer.WriteInfo("EXCEPTION OCCURRED:")
                                 .WriteInfo(exception.ToString())
                                 .WriteInfo(String.Empty);

                LogUtility.Writer.WriteError("EXCEPTION OCCURRED:")
                                 .WriteError(exception.ToString())
                                 .WriteError(String.Empty);
            }

            var validationException = exception as HttpRequestValidationException;

            if (validationException != null)
            {
                SetResponseStatus(HttpStatusCode.Forbidden, RestResources.ValidationRequestFailed);
                return;
            }

            var httpException = exception as HttpException;

            if (httpException != null)
            {
                if (httpException.Message.Contains(DangerousRequestMessage))
                {
                    SetResponseStatus(HttpStatusCode.Forbidden, RestResources.ValidationRequestFailed);
                }
                else if (httpException.Message.Contains(RequestTimeoutMessage))
                {
                    SetResponseStatus(HttpStatusCode.ServiceUnavailable, RestResources.ServiceTimedOut);
                }
            }
        }

        private void TryClearCompressionFilter()
        {
            try
            {
                if (m_context.Response.Filter != null && m_context.Response.Filter.GetType().Namespace != typeof(HttpResponse).Namespace)
                {
                    m_context.Response.Filter = null;
                }
            }
            catch (Exception)
            {
            }
        }

        private void IngestPageDependencies()
        {
            var handler = m_context.Context.CurrentHandler as Page;

            if (handler == null)
            {
                return;
            }

            WebFormsInjectionHelper.InjectControlDependencies(handler);

            handler.PreInit += (s, e) => WebFormsInjectionHelper.InitializeChildControls(handler);
        }

        private void RemoveServerHeaders()
        {
            if (!m_integratedPipeline)
            {
                return;
            }

            m_context.Response.Headers.Remove("Server");
            m_context.Response.Headers.Remove("X-AspNet-Version");
            m_context.Response.Headers.Remove("X-Powered-By");
        }

        private void SetResponseHeaders()
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

                m_context.Response.AppendHeader(header.Key, header.Value);
            }
        }

        private void SetFaultResponse(HttpResourceFaultException faultException)
        {
            m_context.Server.ClearError();
            m_context.Response.Clear();
            m_context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            m_context.Response.StatusDescription = RestResources.ResourceValidationFailed;

            if ("POST".Equals(m_context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase) || "PUT".Equals(m_context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase) ||
                "PATCH".Equals(m_context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                m_context.Response.TrySkipIisCustomErrors = true;

                OutputResourceValidationFaults(faultException);
            }

            m_context.CompleteRequest();
        }
    }
}
