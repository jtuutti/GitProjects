// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Web.Routing;
using RestFoundation.Formatters;
using RestFoundation.Runtime;
using RestFoundation.Security;

namespace RestFoundation.Configuration
{
    /// <summary>
    /// Represents the REST Foundation configuration options.
    /// </summary>
    public sealed class RestOptions
    {
        internal RestOptions()
        {
            FaultDetail = FaultDetail.DetailedInDebugMode;
            JsonSettings = new JsonFormatterSettings();

            BeginRequestAction = context => { };
            EndRequestAction = context => { };
            ExceptionAction = (context, Exception) => { };
        }

        /// <summary>
        /// Gets a value indicating whether the service proxy must be accessed through the HTTPS protocol only.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ServiceProxyHttpsOnly { get; internal set; }

        /// <summary>
        /// Gets the service proxy authorization manager instance.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IAuthorizationManager ServiceProxyAuthorizationManager { get; internal set; }

        /// <summary>
        /// Gets the service description.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ServiceDescription { get; internal set; }

        internal bool IsServiceProxyInitialized { get; set; }
        internal string ServiceProxyRelativeUrl { get; set; }
        internal string DefaultMediaType { get; private set; }
        internal bool ForceDefaultMediaType { get; private set; }
        internal FaultDetail FaultDetail { get; private set; }
        internal bool RetainWebServerHeaders { get; private set; }
        internal IDictionary<string, string> ResponseHeaders { get; private set; }
        internal string IndexPageRelativeUrl { get; private set; }
        internal JsonFormatterSettings JsonSettings { get; private set; }
        internal XmlFormatterSettings XmlSettings { get; private set; }
        internal Action<IServiceContext> BeginRequestAction { get; private set; }
        internal Action<IServiceContext> EndRequestAction { get; private set; }
        internal Action<IServiceContext, Exception> ExceptionAction { get; private set; }

        /// <summary>
        /// Calls the provided service proxy configuration object to set up service help and proxy UI for the services.
        /// </summary>
        /// <param name="configuration">The service help and proxy configuration.</param>
        /// <returns>The configuration options object.</returns>
        public RestOptions ConfigureServiceHelpAndProxy(Action<ProxyConfiguration> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            configuration(new ProxyConfiguration());
            return this;
        }

        /// <summary>
        /// Adds media type formatters for the JSONP support.
        /// </summary>
        /// <returns>The configuration options object.</returns>
        public RestOptions EnableJsonPSupport()
        {
            MediaTypeFormatterRegistry.SetFormatter("application/javascript", new JsonPFormatter());
            MediaTypeFormatterRegistry.SetFormatter("text/javascript", new JsonPFormatter());

            return this;
        }

        /// <summary>
        /// Sets the default media type in case the Accept HTTP header is not provided or should
        /// be ignored.
        /// </summary>
        /// <param name="mediaType">The media type.</param>
        /// <param name="forceDefaultMediaType">
        /// Set to true to always use the provided media type regardless of the Accept HTTP header value;
        /// otherwise set to false.
        /// </param>
        /// <returns>The configuration options object.</returns>
        public RestOptions WithDefaultMediaType(string mediaType, bool forceDefaultMediaType)
        {
            if (String.IsNullOrEmpty(mediaType))
            {
                throw new ArgumentNullException("mediaType");
            }

            DefaultMediaType = mediaType;
            ForceDefaultMediaType = forceDefaultMediaType;

            return this;
        }

        /// <summary>
        /// Sets the detail of information being returned in the fault collection object during an unhandled service
        /// exception.
        /// </summary>
        /// <param name="detail">The detail of fault information.</param>
        /// <returns>The configuration options object.</returns>
        public RestOptions WithFaultDetail(FaultDetail detail)
        {
            if (!Enum.IsDefined(typeof(FaultDetail), detail))
            {
                throw new ArgumentOutOfRangeException("detail");
            }

            FaultDetail = detail;
            return this;
        }

        /// <summary>
        /// Calls the provided global behavior builder delegate to set or remove behaviors global to all services.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The configuration options object.</returns>
        public RestOptions WithGlobalBehaviors(Action<GlobalBehaviorBuilder> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            builder(new GlobalBehaviorBuilder());
            return this;
        }

        /// <summary>
        /// Sets the exception handler global to all services.
        /// </summary>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <returns>The configuration options object.</returns>
        public RestOptions WithGlobalExceptionHandler(IServiceExceptionHandler exceptionHandler)
        {
            if (exceptionHandler == null)
            {
                throw new ArgumentNullException("exceptionHandler");
            }

            ServiceExceptionHandlerRegistry.SetGlobalExceptionHandler(exceptionHandler);
            return this;
        }

        /// <summary>
        /// Sets the default page file name. The file must be in the root folder and only the file name must be
        /// provided.
        /// </summary>
        /// <param name="filename">The file name.</param>
        /// <returns>The configuration options object.</returns>
        /// <exception cref="ArgumentException">
        /// If the file has an unsupported extension or a file path had been provided in addition to the name.
        /// </exception>
        public RestOptions WithIndexFileName(string filename)
        {
            if (String.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename");
            }

            if (filename.IndexOf('~') >= 0 || filename.IndexOf('/') >= 0 || filename.IndexOf('\\') >= 0 || filename.IndexOf(':') >= 0)
            {
                throw new ArgumentException(Resources.Global.FileNameContainsPath);
            }

            string extension = Path.GetExtension(filename);

            if (!String.Equals(extension, ".htm", StringComparison.OrdinalIgnoreCase) &&
                !String.Equals(extension, ".html", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(Resources.Global.InvalidIndexFileException);
            }

            IndexPageRelativeUrl = "~/" + filename.Trim();
            return this;
        }

        /// <summary>
        /// Calls the provided media type formatter builder delegate to set or remove formatters.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The configuration options object.</returns>
        public RestOptions WithMediaTypeFormatters(Action<MediaTypeFormatterBuilder> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            builder(new MediaTypeFormatterBuilder());
            return this;
        }

        /// <summary>
        /// Adds the provided header to all HTTP responses.
        /// </summary>
        /// <param name="headerName">The header name.</param>
        /// <param name="headerValue">The header value.</param>
        /// <returns>The configuration options object.</returns>
        public RestOptions WithResponseHeader(string headerName, string headerValue)
        {
            if (String.IsNullOrEmpty(headerName))
            {
                throw new ArgumentNullException("headerName");
            }

            if (headerValue == null)
            {
                throw new ArgumentNullException("headerValue");
            }

            if (ResponseHeaders == null)
            {
                ResponseHeaders = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { headerName, headerValue }
                };
            }

            ResponseHeaders[headerName] = headerValue;
            return this;
        }

        /// <summary>
        /// Adds the provided response headers to all HTTP responses.
        /// </summary>
        /// <param name="responseHeaders">A dictionary of header names and values.</param>
        /// <returns>The configuration options object.</returns>
        public RestOptions WithResponseHeaders(IDictionary<string, string> responseHeaders)
        {
            if (responseHeaders == null)
            {
                throw new ArgumentNullException("responseHeaders");
            }

            if (ResponseHeaders != null)
            {
                foreach (KeyValuePair<string, string> header in responseHeaders)
                {
                    ResponseHeaders[header.Key] = header.Value;
                }
            }
            else
            {
                ResponseHeaders = new SortedDictionary<string, string>(responseHeaders, StringComparer.OrdinalIgnoreCase);
            }

            return this;
        }

        /// <summary>
        /// Calls the provided type binder builder delegate to set or remove type binders.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The configuration options object.</returns>
        public RestOptions WithTypeBinders(Action<TypeBinderBuilder> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            builder(new TypeBinderBuilder());
            return this;
        }

        /// <summary>
        /// Calls the provided URL builder delegate to set up URL routes to services and web form pages.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The configuration options object.</returns>
        public RestOptions WithUrls(Action<UrlBuilder> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            builder(new UrlBuilder(RouteTable.Routes));
            return this;
        }

        /// <summary>
        /// Sets custom JSON formatter and result settings.
        /// </summary>
        /// <param name="settings">The JSON formatter settings.</param>
        /// <returns>The configuration options object.</returns>
        public RestOptions WithJsonFormatterSettings(JsonFormatterSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            JsonSettings = settings;
            return this;
        }

        /// <summary>
        /// Sets custom XML formatter and result settings.
        /// </summary>
        /// <param name="settings">The XML formatter settings.</param>
        /// <returns>The configuration options object.</returns>
        public RestOptions WithXmlFormatterSettings(XmlFormatterSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            XmlSettings = settings;
            return this;
        }

        /// <summary>
        /// Retains the HTTP response headers generated by the IIS server and ASP .NET framework.
        /// It is not recommended to retain these headers because they expose the technology
        /// the services are implemented in to the user.
        /// </summary>
        /// <returns>The configuration options object.</returns>
        public RestOptions DoNotRemoveWebServerHeaders()
        {
            RetainWebServerHeaders = true;
            return this;
        }

        /// <summary>
        /// Sets an action to execute when a REST service request begins.
        /// </summary>
        /// <param name="action">The action delegate.</param>
        /// <returns>The configuration options object.</returns>
        public RestOptions OnBeginRequest(Action<IServiceContext> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            BeginRequestAction = action;
            return this;
        }

        /// <summary>
        /// Sets an action to execute when a REST service request ends.
        /// </summary>
        /// <param name="action">The action delegate.</param>
        /// <returns>The configuration options object.</returns>
        public RestOptions OnEndRequest(Action<IServiceContext> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            EndRequestAction = action;
            return this;
        }

        /// <summary>
        /// Sets an action to execute when a REST service method throws an exception.
        /// </summary>
        /// <param name="action">The action delegate.</param>
        /// <returns>The configuration options object.</returns>
        public RestOptions OnException(Action<IServiceContext, Exception> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            ExceptionAction = action;
            return this;
        }
    }
}
