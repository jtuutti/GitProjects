﻿// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.UI;
using RestFoundation.Runtime;

namespace RestFoundation
{
    /// <summary>
    /// Represents an HTTP module required by the REST foundation.
    /// </summary>
    public sealed class RestHttpModule : IHttpModule
    {
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

            m_integratedPipeline = HttpRuntime.UsingIntegratedPipeline;

            context.Error += (sender, args) => CompleteRequestOnError(context);
            context.PreRequestHandlerExecute += (sender, args) => IngestPageDependencies(context);
            context.PreSendRequestHeaders += (sender, args) =>
            {
                RemoveServerHeaders(context);
                SetResponseHeaders(context);
            };
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose()
        {
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

            if (httpException != null)
            {
                if (httpException.Message.Contains("A potentially dangerous Request.Path value was detected from the client"))
                {
                    SetResponseStatus(context, HttpStatusCode.Forbidden, "A potentially dangerous value was found in the HTTP request");
                }
                else if (httpException.Message.Contains("Request timed out"))
                {
                    SetResponseStatus(context, HttpStatusCode.ServiceUnavailable, "Service timed out");
                }
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

        private static void IngestPageDependencies(HttpApplication context)
        {
            var handler = context.Context.CurrentHandler as Page;

            if (handler != null)
            {
                InjectControlDependencies(handler);

                handler.PreInit += (s, e) => InitializeChildControls(handler);
            }
        }

        private static void InjectControlDependencies(Control control)
        {
            Type controlType = control.GetType().BaseType;

            if (controlType != null)
            {
                ConstructorInfo constructor = (from ctor in controlType.GetConstructors()
                                               let parameterLength = ctor.GetParameters().Length
                                               where parameterLength > 0
                                               orderby parameterLength descending
                                               select ctor).FirstOrDefault();

                if (constructor != null)
                {
                    CallPageInjectionConstructor(control, constructor);
                }
            }
        }

        private static void CallPageInjectionConstructor(Control control, ConstructorInfo constructor)
        {
            var parameters = from parameter in constructor.GetParameters()
                             let parameterType = parameter.ParameterType
                             select Rest.Configuration.ServiceLocator.GetService(parameterType);

            constructor.Invoke(control, parameters.ToArray());
        }

        private static void InitializeChildControls(Control control)
        {
            var childControls = control.Controls.OfType<UserControl>();

            foreach (var childControl in childControls)
            {
                InjectControlDependencies(childControl);
                InitializeChildControls(childControl);
            }
        }

        private static void SetResponseHeaders(HttpApplication context)
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

                context.Response.AppendHeader(header.Key, header.Value);
            }
        }

        private void RemoveServerHeaders(HttpApplication context)
        {
            if (!m_integratedPipeline)
            {
                return;
            }

            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-AspNet-Version");
            context.Response.Headers.Remove("X-Powered-By");
        }
    }
}
