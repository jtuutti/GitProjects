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
    public class HttpRequest : RestContextBase, IHttpRequest
    {
        private const string AjaxHeaderName = "X-Requested-With";
        private const string AjaxHeaderValue = "XMLHttpRequest";
        private const string FormDataMediaType = "application/x-www-form-urlencoded";
        private const string ForwardedProtocolHeaderName = "X-Forwarded-Proto";
        private const string MultiFormDataMediaType = "multipart/form-data";

        private readonly object m_uriSyncRoot = new Object();
        private readonly object m_methodSyncRoot = new Object();
        private readonly object m_resourceBagSyncRoot = new Object();
        private readonly object m_headerCollectionSyncRoot = new Object();
        private readonly object m_cookieCollectionSyncRoot = new Object();
        private readonly object m_routeCollectionSyncRoot = new Object();
        private readonly object m_queryStringCollectionSyncRoot = new Object();
        private readonly object m_formCollectionSyncRoot = new Object();
        private readonly object m_serverVariablesCollectionSyncRoot = new Object();

        private readonly RestContextContainer m_contextContainer;
        private DynamicDictionary m_resourceBag;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequest"/> class.
        /// </summary>
        public HttpRequest()
        {
            m_contextContainer = new RestContextContainer(() => Context.Items);
        }

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
                if (m_contextContainer.Uri != null)
                {
                    return m_contextContainer.Uri;
                }

                lock (m_uriSyncRoot)
                {
                    return m_contextContainer.Uri ?? (m_contextContainer.Uri = new ServiceOperationUri(Context.Request.Url, Context.Request.ApplicationPath));
                }
            }
        }

        /// <summary>
        /// Gets the HTTP method of the request.
        /// </summary>
        public HttpMethod Method
        {
            get
            {
                if (m_contextContainer.Method.HasValue)
                {
                    return m_contextContainer.Method.Value;
                }

                lock (m_methodSyncRoot)
                {
                    return m_contextContainer.Method.HasValue ? m_contextContainer.Method.Value : (m_contextContainer.Method = Context.GetOverriddenHttpMethod()).Value;
                }
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
                if (m_resourceBag != null)
                {
                    return m_resourceBag;
                }

                lock (m_resourceBagSyncRoot)
                {
                    if (m_resourceBag != null)
                    {
                        return m_resourceBag;
                    }

                    dynamic resourceDictionary = new DynamicDictionary(() => Context.Items);
                    resourceDictionary.GetResource = new Func<dynamic>(() => GetResource());

                    m_resourceBag = resourceDictionary;
                    return resourceDictionary;
                }
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
        public IRouteValueCollection RouteValues
        {
            get
            {
                if (m_contextContainer.RouteValues != null)
                {
                    return m_contextContainer.RouteValues;
                }

                lock (m_routeCollectionSyncRoot)
                {
                    if (m_contextContainer.RouteValues != null)
                    {
                        return m_contextContainer.RouteValues;
                    }

                    IRouteValueCollection routeCollection = new RouteValueCollection(GetRouteValues());
                    m_contextContainer.RouteValues = routeCollection;

                    return routeCollection;
                }
            }
        }

        /// <summary>
        /// Gets the request header collection.
        /// </summary>
        public IHeaderCollection Headers
        {
            get
            {
                if (m_contextContainer.Headers != null)
                {
                    return m_contextContainer.Headers;
                }

                lock (m_headerCollectionSyncRoot)
                {
                    if (m_contextContainer.Headers != null)
                    {
                        return m_contextContainer.Headers;
                    }

                    m_contextContainer.Headers = new HeaderCollection(IsUnvalidatedRequest ? Context.Request.Unvalidated.Headers : Context.Request.Headers);
                    return m_contextContainer.Headers;
                }
            }
        }

        /// <summary>
        /// Gets the query string collection.
        /// </summary>
        public IStringValueCollection QueryString
        {
            get
            {
                if (m_contextContainer.QueryString != null)
                {
                    return m_contextContainer.QueryString;
                }

                lock (m_queryStringCollectionSyncRoot)
                {
                    if (m_contextContainer.QueryString != null)
                    {
                        return m_contextContainer.QueryString;
                    }

                    m_contextContainer.QueryString = new StringValueCollection(IsUnvalidatedRequest ? Context.Request.Unvalidated.QueryString : Context.Request.QueryString);
                    return m_contextContainer.QueryString;
                }
            }
        }

        /// <summary>
        /// Gets an uploaded file collection.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations",
                         Justification = "This is a special type of exception that will be handled by the HTTP module.")]
        public IUploadedFileCollection Files
        {
            get
            {
                if (Context.Request.ContentType == null || Context.Request.ContentType.IndexOf(MultiFormDataMediaType, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest, Resources.Global.UnsupportedMultiFormData);
                }

                var files = ServiceRequestValidator.IsUnvalidatedRequest(Context) ? Context.Request.Unvalidated.Files : Context.Request.Files;

                return new UploadedFileCollection(files);
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
                if (Context.Request.ContentType == null || Context.Request.ContentType.IndexOf(FormDataMediaType, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest, Resources.Global.UnsupportedFormData);
                }

                if (m_contextContainer.Form != null)
                {
                    return m_contextContainer.Form;
                }

                lock (m_formCollectionSyncRoot)
                {
                    if (m_contextContainer.Form != null)
                    {
                        return m_contextContainer.Form;
                    }

                    m_contextContainer.Form = new StringValueCollection(IsUnvalidatedRequest ? Context.Request.Unvalidated.Form : Context.Request.Form);
                    return m_contextContainer.Form;
                }
            }
        }

        /// <summary>
        /// Gets the server variable collection.
        /// </summary>
        public IServerVariableCollection ServerVariables
        {
            get
            {
                if (m_contextContainer.ServerVariables != null)
                {
                    return m_contextContainer.ServerVariables;
                }

                lock (m_serverVariablesCollectionSyncRoot)
                {
                    if (m_contextContainer.ServerVariables != null)
                    {
                        return m_contextContainer.ServerVariables;
                    }

                    m_contextContainer.ServerVariables = new ServerVariableCollection(Context.Request.ServerVariables);
                    return m_contextContainer.ServerVariables;
                }
            }
        }

        /// <summary>
        /// Gets the request cookie collection.
        /// </summary>
        public ICookieValueCollection Cookies
        {
            get
            {
                if (m_contextContainer.Cookies != null)
                {
                    return m_contextContainer.Cookies;
                }

                lock (m_cookieCollectionSyncRoot)
                {
                    if (m_contextContainer.Cookies != null)
                    {
                        return m_contextContainer.Cookies;
                    }

                    m_contextContainer.Cookies = new CookieValueCollection(IsUnvalidatedRequest ? Context.Request.Unvalidated.Cookies : Context.Request.Cookies);
                    return m_contextContainer.Cookies;
                }
            }
        }
        
        private bool IsUnvalidatedRequest
        {
            get
            {
                return ServiceRequestValidator.IsUnvalidatedRequest(Context);
            }
        }

        private RouteValueDictionary GetRouteValues()
        {
            RouteData routeData = RouteTable.Routes.GetRouteData(Context);

            return (routeData != null && routeData.Values != null) ? routeData.Values : new RouteValueDictionary();
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

            if (formatter == null || !formatter.CanFormatRequest)
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
    }
}
