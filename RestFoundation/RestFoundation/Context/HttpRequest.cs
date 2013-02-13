// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Web.Routing;
using RestFoundation.Collections;
using RestFoundation.Collections.Concrete;
using RestFoundation.Collections.Specialized;
using RestFoundation.Formatters;
using RestFoundation.Runtime;
using RestFoundation.Validation;

namespace RestFoundation.Context
{
    /// <summary>
    /// Represents an HTTP request.
    /// </summary>
    public class HttpRequest : ContextBase, IHttpRequest
    {
        private const string AjaxHeaderName = "X-Requested-With";
        private const string AjaxHeaderValue = "XMLHttpRequest";
        private const string ContextContainerKey = "REST_Context";
        private const string FormDataMediaType = "application/x-www-form-urlencoded";
        private const string ForwardedProtocolHeaderName = "X-Forwarded-Proto";

        /// <summary>
        /// Gets a value indicating whether the request was initiated through AJAX.
        /// </summary>
        public bool IsAjax
        {
            get
            {
                return String.Equals(AjaxHeaderValue, Headers.TryGet(AjaxHeaderName), StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the request was initiated from the local computer.
        /// </summary>
        public bool IsLocal
        {
            get
            {
                return Context.Request.IsLocal;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the HTTP connection uses SSL (HTTPS protocol).
        /// </summary>
        public bool IsSecure
        {
            get
            {
                return Context.Request.IsSecureConnection || String.Equals("https", Headers.TryGet(ForwardedProtocolHeaderName), StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Gets the service operation URL.
        /// </summary>
        public ServiceOperationUri Url
        {
            get
            {
                return ContextContainer.Url ?? (ContextContainer.Url = new ServiceOperationUri(Context.Request.Url, Context.Request.ApplicationPath));
            }
        }

        /// <summary>
        /// Gets the HTTP method of the request.
        /// </summary>
        public HttpMethod Method
        {
            get
            {
                if (!ContextContainer.Method.HasValue)
                {
                    ContextContainer.Method = Context.GetOverriddenHttpMethod();
                }

                return ContextContainer.Method.Value;
            }
        }

        /// <summary>
        /// Gets the stream containing the HTTP request body data.
        /// </summary>
        public Stream Body
        {
            get
            {
                return Context.Request.InputStream;
            }
        }

        /// <summary>
        /// Gets the dynamic resource object bag.
        /// </summary>
        public dynamic ResourceBag
        {
            get
            {
                dynamic resourceDictionary = new DynamicDictionary(() => Context.Items);
                resourceDictionary.GetResource = new Func<dynamic>(() => GetResource());

                return resourceDictionary;
            }
        }

        /// <summary>
        /// Gets a resource state associated with the service method with associated validation errors.
        /// </summary>
        public ResourceState ResourceState
        {
            get
            {
                var validationErrors = Context.Items[ResourceValidator.ValidationErrorKey] as ResourceState;

                return validationErrors ?? new ResourceState(new ValidationError[0]);
            }
        }

        /// <summary>
        /// Gets the route collection.
        /// </summary>
        public IObjectValueCollection RouteValues
        {
            get
            {
                return ContextContainer.RouteValues ?? (ContextContainer.RouteValues = GenerateRouteValues());
            }
        }

        /// <summary>
        /// Gets the request header collection.
        /// </summary>
        public IHeaderCollection Headers
        {
            get
            {
                return ContextContainer.Headers ?? (ContextContainer.Headers = new HeaderCollection(Context.Request.Headers));
            }
        }

        /// <summary>
        /// Gets the query string collection.
        /// </summary>
        public IStringValueCollection QueryString
        {
            get
            {
                return ContextContainer.QueryString ?? (ContextContainer.QueryString = new StringValueCollection(Context.Request.QueryString));
            }
        }

        /// <summary>
        /// Gets the form name/value collection.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations",
                         Justification = "This is a special type of exception that will be handled by the HTTP module.")]
        public IStringValueCollection Form
        {
            get
            {
                if (ContextContainer.Form == null &&
                    (Context.Request.ContentType == null || Context.Request.ContentType.IndexOf(FormDataMediaType, StringComparison.OrdinalIgnoreCase) < 0))
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError, RestResources.UnsupportedFormData);
                }

                return ContextContainer.Form ?? (ContextContainer.Form = new StringValueCollection(Context.Request.Form));
            }
        }

        /// <summary>
        /// Gets the server variable collection.
        /// </summary>
        public IServerVariableCollection ServerVariables
        {
            get
            {
                return ContextContainer.ServerVariables ?? (ContextContainer.ServerVariables = new ServerVariableCollection(Context.Request.ServerVariables));
            }
        }

        /// <summary>
        /// Gets the request cookie collection.
        /// </summary>
        public ICookieValueCollection Cookies
        {
            get
            {
                return ContextContainer.Cookies ?? (ContextContainer.Cookies = new CookieValueCollection(Context.Request.Cookies));
            }
        }

        private HttpRequestContainer ContextContainer
        {
            get
            {
                lock (Context.Items)
                {
                    var items = Context.Items[ContextContainerKey] as HttpRequestContainer;

                    if (items == null)
                    {
                        items = new HttpRequestContainer();
                        Context.Items[ContextContainerKey] = items;
                    }

                    return items;
                }
            }
        }

        private IObjectValueCollection GenerateRouteValues()
        {
            var routeData = RouteTable.Routes.GetRouteData(Context);

            return routeData != null ? new ObjectValueCollection(routeData.Values) : new ObjectValueCollection(new RouteValueDictionary());
        }

        private dynamic GetResource()
        {
            if (Method != HttpMethod.Post && Method != HttpMethod.Put && Method != HttpMethod.Patch)
            {
                return null;
            }

            if (String.IsNullOrWhiteSpace(Headers.ContentType) || Body.Length == 0)
            {
                return null;
            }

            IMediaTypeFormatter formatter = MediaTypeFormatterRegistry.GetFormatter(Headers.ContentType);

            if (formatter == null || formatter is BlockFormatter)
            {
                return null;
            }

            dynamic resource;

            try
            {
                resource = formatter.FormatRequest(Rest.Configuration.ServiceLocator.GetService<IServiceContext>(), typeof(object));
            }
            catch (Exception)
            {
                resource = null;
            }

            return resource;
        }

        private sealed class HttpRequestContainer
        {
            public ServiceOperationUri Url { get; set; }
            public HttpMethod? Method { get; set; }

            public IObjectValueCollection RouteValues { get; set; }
            public IHeaderCollection Headers { get; set; }
            public IStringValueCollection QueryString { get; set; }
            public IStringValueCollection Form { get; set; }
            public IServerVariableCollection ServerVariables { get; set; }
            public ICookieValueCollection Cookies { get; set; }
        }
    }
}
