using System;
using System.IO;
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

        private static HttpContext Context
        {
            get
            {
                return HttpContext.Current;
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

        public bool IsAjax
        {
            get
            {
                return String.Equals(AjaxHeaderValue, Headers.TryGet(AjaxHeaderName), StringComparison.OrdinalIgnoreCase);
            }
        }

        public bool IsLocal
        {
            get
            {
                return Context.Request.IsLocal;
            }
        }

        public bool IsSecure
        {
            get
            {
                return Context.Request.IsSecureConnection;
            }
        }

        public ServiceUri Url
        {
            get
            {
                if (ContextContainer.Url == null)
                {
                    ContextContainer.Url = new ServiceUri(Context.Request.Url, GenerateServiceUrl());
                }

                return ContextContainer.Url;
            }
        }

        public Stream Body
        {
            get
            {
                return Context.Request.InputStream;
            }
        }

        public HttpMethod Method
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

        public dynamic QueryBag
        {
            get
            {
                if (ContextContainer.Query == null)
                {
                    ContextContainer.Query = new DynamicStringCollection(QueryString);
                }

                return ContextContainer.Query;
            }
        }

        public IObjectValueCollection RouteValues
        {
            get
            {
                if (ContextContainer.RouteValues == null)
                {
                    ContextContainer.RouteValues = GenerateRouteValues();
                }

                return ContextContainer.RouteValues;
            }
        }

        public IHeaderCollection Headers
        {
            get
            {
                if (ContextContainer.Headers == null)
                {
                    ContextContainer.Headers = new HeaderCollection(Context.Request.Headers);
                }

                return ContextContainer.Headers;
            }
        }

        public IStringValueCollection QueryString
        {
            get
            {
                if (ContextContainer.QueryString == null)
                {
                    ContextContainer.QueryString = new StringValueCollection(Context.Request.QueryString);
                }

                return ContextContainer.QueryString;
            }
        }

        public IStringValueCollection ServerVariables
        {
            get
            {
                if (ContextContainer.ServerVariables == null)
                {
                    ContextContainer.ServerVariables = new StringValueCollection(Context.Request.ServerVariables);
                }

                return ContextContainer.ServerVariables;
            }
        }

        public ICookieValueCollection Cookies
        {
            get
            {
                if (ContextContainer.Cookies == null)
                {
                    ContextContainer.Cookies = new CookieValueCollection(Context.Request.Cookies);
                }

                return ContextContainer.Cookies;
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
