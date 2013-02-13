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
        private const string FormDataMediaType = "application/x-www-form-urlencoded";
        private const string ForwardedProtocolHeaderName = "X-Forwarded-Proto";

        private readonly object m_uriSyncRoot = new Object();
        private readonly object m_methodSyncRoot = new Object();
        private readonly object m_resourceBagSyncRoot = new Object();
        private readonly object m_headerCollectionSyncRoot = new Object();
        private readonly object m_cookieCollectionSyncRoot = new Object();
        private readonly object m_routeCollectionSyncRoot = new Object();
        private readonly object m_queryStringCollectionSyncRoot = new Object();
        private readonly object m_formCollectionSyncRoot = new Object();
        private readonly object m_serverVariablesCollectionSyncRoot = new Object();

        private ServiceOperationUri m_uri;
        private HttpMethod? m_method;
        private DynamicDictionary m_resourceBag;
        private IHeaderCollection m_headerCollection;
        private ICookieValueCollection m_cookieCollection;
        private IRouteValueCollection m_routeCollection;
        private IStringValueCollection m_queryStringCollection, m_formCollection;
        private IServerVariableCollection m_serverVariableCollection;

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
                if (m_uri != null)
                {
                    return m_uri;
                }

                lock (m_uriSyncRoot)
                {
                    return m_uri ?? (m_uri = new ServiceOperationUri(Context.Request.Url, Context.Request.ApplicationPath));
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
                if (m_method.HasValue)
                {
                    return m_method.Value;
                }

                lock (m_methodSyncRoot)
                {
                    return m_method.HasValue ? m_method.Value : (m_method = Context.GetOverriddenHttpMethod()).Value;
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
                if (m_routeCollection != null)
                {
                    return m_routeCollection;
                }

                lock (m_routeCollectionSyncRoot)
                {
                    if (m_routeCollection != null)
                    {
                        return m_routeCollection;
                    }

                    m_routeCollection = new RouteValueCollection(GetRouteValues());
                    return m_routeCollection;
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
                if (m_headerCollection != null)
                {
                    return m_headerCollection;
                }

                lock (m_headerCollectionSyncRoot)
                {
                    if (m_headerCollection != null)
                    {
                        return m_headerCollection;
                    }

                    m_headerCollection = new HeaderCollection(Context.Request.Headers);
                    return m_headerCollection;
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
                if (m_queryStringCollection != null)
                {
                    return m_queryStringCollection;
                }

                lock (m_queryStringCollectionSyncRoot)
                {
                    if (m_queryStringCollection != null)
                    {
                        return m_queryStringCollection;
                    }

                    m_queryStringCollection = new StringValueCollection(Context.Request.QueryString);
                    return m_queryStringCollection;
                }
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
                    throw new HttpResponseException(HttpStatusCode.InternalServerError, RestResources.UnsupportedFormData);
                }

                if (m_formCollection != null)
                {
                    return m_formCollection;
                }

                lock (m_formCollectionSyncRoot)
                {
                    if (m_formCollection != null)
                    {
                        return m_formCollection;
                    }

                    m_formCollection = new StringValueCollection(Context.Request.Form);
                    return m_formCollection;
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
                if (m_serverVariableCollection != null)
                {
                    return m_serverVariableCollection;
                }

                lock (m_serverVariablesCollectionSyncRoot)
                {
                    if (m_serverVariableCollection != null)
                    {
                        return m_serverVariableCollection;
                    }

                    m_serverVariableCollection = new ServerVariableCollection(Context.Request.ServerVariables);
                    return m_serverVariableCollection;
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
                if (m_cookieCollection != null)
                {
                    return m_cookieCollection;
                }

                lock (m_cookieCollectionSyncRoot)
                {
                    if (m_cookieCollection != null)
                    {
                        return m_cookieCollection;
                    }

                    m_cookieCollection = new CookieValueCollection(Context.Request.Cookies);
                    return m_cookieCollection;
                }
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
    }
}
