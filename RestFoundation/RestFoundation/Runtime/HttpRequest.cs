using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Routing;
using RestFoundation.Collections;
using RestFoundation.Collections.Concrete;
using RestFoundation.Collections.Specialized;

namespace RestFoundation.Runtime
{
    public class HttpRequest : IHttpRequest
    {
        private const string AjaxHeaderName = "X-Requested-With";
        private const string AjaxHeaderValue = "XMLHttpRequest";
        private const string ContextContainerKey = "REST_Context";

        private static readonly object syncRoot = new object();
        private readonly ICredentialResolver m_credentialResolver;

        public HttpRequest(ICredentialResolver credentialResolver)
        {
            if (credentialResolver == null) throw new ArgumentNullException("credentialResolver");

            m_credentialResolver = credentialResolver;
        }

        private static HttpContext Context
        {
            get
            {
                HttpContext context = HttpContext.Current;

                if (context == null)
                {
                    throw new InvalidOperationException("No HTTP context was found");
                }

                return context;
            }
        }

        private static HttpRequestContainer ContextContainer
        {
            get
            {
                var items = Context.Items[ContextContainerKey] as HttpRequestContainer;

                if (items == null)
                {
                    lock (syncRoot)
                    {
                        items = Context.Items[ContextContainerKey] as HttpRequestContainer;

                        if (items == null)
                        {
                            items = new HttpRequestContainer();
                            Context.Items[ContextContainerKey] = items;
                        }
                    }
                }

                return items;
            }
        }

        public virtual bool IsAjax
        {
            get
            {
                return String.Equals(AjaxHeaderValue, Headers.TryGet(AjaxHeaderName), StringComparison.OrdinalIgnoreCase);
            }
        }

        public virtual bool IsLocal
        {
            get
            {
                return Context.Request.IsLocal;
            }
        }

        public virtual bool IsSecure
        {
            get
            {
                return Context.Request.IsSecureConnection;
            }
        }

        public virtual ServiceUri Url
        {
            get
            {
                return ContextContainer.Url ?? (ContextContainer.Url = new ServiceUri(Context.Request.Url, GenerateServiceUrl()));
            }
        }

        public virtual HttpMethod Method
        {
            get
            {
                if (!ContextContainer.Method.HasValue)
                {
                    ContextContainer.Method = HttpContext.Current.GetOverriddenHttpMethod();
                }

                return ContextContainer.Method.Value;
            }
        }

        public virtual Stream Body
        {
            get
            {
                return Context.Request.InputStream;
            }
        }

        public virtual NetworkCredential Credentials
        {
            get
            {
                return m_credentialResolver.GetCredentials(this);
            }
        }

        public virtual dynamic QueryBag
        {
            get
            {
                return ContextContainer.Query ?? (ContextContainer.Query = new DynamicStringCollection(QueryString));
            }
        }

        public virtual IObjectValueCollection RouteValues
        {
            get
            {
                return ContextContainer.RouteValues ?? (ContextContainer.RouteValues = GenerateRouteValues());
            }
        }

        public virtual IHeaderCollection Headers
        {
            get
            {
                return ContextContainer.Headers ?? (ContextContainer.Headers = new HeaderCollection(Context.Request.Headers));
            }
        }

        public virtual IStringValueCollection QueryString
        {
            get
            {
                return ContextContainer.QueryString ?? (ContextContainer.QueryString = new StringValueCollection(Context.Request.QueryString));
            }
        }

        public virtual IStringValueCollection ServerVariables
        {
            get
            {
                return ContextContainer.ServerVariables ?? (ContextContainer.ServerVariables = new StringValueCollection(Context.Request.ServerVariables));
            }
        }

        public virtual ICookieValueCollection Cookies
        {
            get
            {
                return ContextContainer.Cookies ?? (ContextContainer.Cookies = new CookieValueCollection(Context.Request.Cookies));
            }
        }

        private static IObjectValueCollection GenerateRouteValues()
        {
            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(Context));

            return routeData != null ? new ObjectValueCollection(routeData.Values) : new ObjectValueCollection(new RouteValueDictionary());
        }

        private string GenerateServiceUrl()
        {
            var serviceUrl = RouteValues.TryGet(RouteConstants.ServiceUrl) as string;
            string endpointUrl = Context.Request.AppRelativeCurrentExecutionFilePath;

            if (serviceUrl == null)
            {
                return endpointUrl;
            }

            return endpointUrl.Substring(0, endpointUrl.IndexOf(serviceUrl, StringComparison.OrdinalIgnoreCase) + serviceUrl.Length);
        }

        private sealed class HttpRequestContainer
        {
            public ServiceUri Url { get; set; }
            public HttpMethod? Method { get; set; }

            public dynamic Query { get; set; }

            public IObjectValueCollection RouteValues { get; set; }
            public IHeaderCollection Headers { get; set; }
            public IStringValueCollection QueryString { get; set; }
            public IStringValueCollection ServerVariables { get; set; }
            public ICookieValueCollection Cookies { get; set; }
        }
    }
}
