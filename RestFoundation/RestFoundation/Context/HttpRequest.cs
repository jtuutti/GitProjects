using System;
using System.IO;
using System.Web.Routing;
using RestFoundation.Collections;
using RestFoundation.Collections.Concrete;
using RestFoundation.Collections.Specialized;

namespace RestFoundation.Context
{
    public class HttpRequest : ContextBase, IHttpRequest
    {
        private const string AjaxHeaderName = "X-Requested-With";
        private const string AjaxHeaderValue = "XMLHttpRequest";
        private const string ContextContainerKey = "REST_Context";

        private HttpRequestContainer ContextContainer
        {
            get
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
                return ContextContainer.Url ?? (ContextContainer.Url = new ServiceUri(Context.Request.Url, Context.Request.ApplicationPath));
            }
        }

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

        public Stream Body
        {
            get
            {
                return Context.Request.InputStream;
            }
        }

        public dynamic QueryBag
        {
            get
            {
                return ContextContainer.Query ?? (ContextContainer.Query = new DynamicStringCollection(QueryString));
            }
        }

        public IObjectValueCollection RouteValues
        {
            get
            {
                return ContextContainer.RouteValues ?? (ContextContainer.RouteValues = GenerateRouteValues());
            }
        }

        public IHeaderCollection Headers
        {
            get
            {
                return ContextContainer.Headers ?? (ContextContainer.Headers = new HeaderCollection(Context.Request.Headers));
            }
        }

        public IStringValueCollection QueryString
        {
            get
            {
                return ContextContainer.QueryString ?? (ContextContainer.QueryString = new StringValueCollection(Context.Request.QueryString));
            }
        }

        public IServerVariableCollection ServerVariables
        {
            get
            {
                return ContextContainer.ServerVariables ?? (ContextContainer.ServerVariables = new ServerVariableCollection(Context.Request.ServerVariables));
            }
        }

        public ICookieValueCollection Cookies
        {
            get
            {
                return ContextContainer.Cookies ?? (ContextContainer.Cookies = new CookieValueCollection(Context.Request.Cookies));
            }
        }

        private IObjectValueCollection GenerateRouteValues()
        {
            var routeData = RouteTable.Routes.GetRouteData(Context);

            return routeData != null ? new ObjectValueCollection(routeData.Values) : new ObjectValueCollection(new RouteValueDictionary());
        }

        private sealed class HttpRequestContainer
        {
            public ServiceUri Url { get; set; }
            public HttpMethod? Method { get; set; }

            public dynamic Query { get; set; }

            public IObjectValueCollection RouteValues { get; set; }
            public IHeaderCollection Headers { get; set; }
            public IStringValueCollection QueryString { get; set; }
            public IServerVariableCollection ServerVariables { get; set; }
            public ICookieValueCollection Cookies { get; set; }
        }
    }
}
