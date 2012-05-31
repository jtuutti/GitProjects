using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace MvcAlt.Infrastructure
{
    public class HttpRequest : IHttpRequest
    {
        private const string HttpMethodOverrideHeader = "X-HTTP-Method-Override";

        private readonly HttpContext context;
        private NameValueCollection values;

        public HttpRequest(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            this.context = context;

            PopulateVerb();
            PopulateRoutes();
        }

        public Uri Url
        {
            get
            {
                return context.Request.Url;
            }
        }

        public string RelativeUrl
        {
            get
            {
                return context.Request.AppRelativeCurrentExecutionFilePath;
            }
        }

        public HttpVerb Verb
        {
            get;
            protected set;
        }

        public Stream Body
        {
            get
            {
                return context.Request.InputStream;
            }
        }

        public RouteValueDictionary RouteValues
        {
            get;
            protected set;
        }

        public NameValueCollection Headers
        {
            get
            {
                return context.Request.Headers;
            }
        }

        public NameValueCollection Query
        {
            get
            {
                return context.Request.QueryString;
            }
        }

        public NameValueCollection Form
        {
            get
            {
                return context.Request.Form;
            }
        }

        public NameValueCollection ServerVariables
        {
            get
            {
                return context.Request.ServerVariables;
            }
        }

        public HttpCookieCollection Cookies
        {
            get
            {
                return context.Request.Cookies;
            }
        }

        public NameValueCollection Values
        {
            get
            {
                if (values == null)
                {
                     PopulateValues();
                }

                return values;
            }
        }

        public string BodyAsString()
        {
            return BodyAsString(null);
        }

        public string BodyAsString(Encoding encoding)
        {
            if (context.Request.InputStream.CanSeek)
            {
                context.Request.InputStream.Seek(0, SeekOrigin.Begin);
            }

            using (var streamReader = new StreamReader(context.Request.InputStream, encoding ?? Encoding.UTF8))
            {
                return streamReader.ReadToEnd();
            }
        }

        public string ToFullUrl(string url)
        {
            if (url == null) throw new ArgumentNullException("url");

            if (Uri.IsWellFormedUriString(url, UriKind.Absolute)) return url;
            if (String.IsNullOrWhiteSpace(url)) { url = "~/"; }

            string[] urlParts = url.Split(new[] { '?' }, 2);
            string baseUrl = urlParts[0];
            
            if (!VirtualPathUtility.IsAbsolute(baseUrl))
            {
                baseUrl = VirtualPathUtility.Combine("~", baseUrl);
            }

            var absoluteUrl = VirtualPathUtility.ToAbsolute(baseUrl);
            
            if (urlParts.Length > 1) absoluteUrl += ("?" + urlParts[1]);
            return absoluteUrl;
        }

        private void PopulateRoutes()
        {
            RouteData routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(context));

            if (routeData == null || routeData.Values == null)
            {
                RouteValues = new RouteValueDictionary();
            }
            else
            {
                RouteValues = routeData.Values;
            }
        }

        private void PopulateVerb()
        {
            string method = context.Request.Headers[HttpMethodOverrideHeader];

            if (String.IsNullOrEmpty(method))
            {
                method = context.Request.QueryString[HttpMethodOverrideHeader];
            }

            if (String.IsNullOrEmpty(method))
            {
                method = context.Request.Form[HttpMethodOverrideHeader];
            }

            if (String.IsNullOrEmpty(method))
            {
                method = context.Request.HttpMethod;
            }

            switch (method.ToUpperInvariant())
            {
                case "HEAD":
                    Verb = HttpVerb.Head;
                    break;
                case "GET":
                    Verb = HttpVerb.Get;
                    break;
                case "POST":
                    Verb = HttpVerb.Post;
                    break;
                case "PUT":
                    Verb = HttpVerb.Put;
                    break;
                case "PATCH":
                    Verb = HttpVerb.Patch;
                    break;
                case "DELETE":
                    Verb = HttpVerb.Delete;
                    break;
                case "OPTIONS":
                    Verb = HttpVerb.Options;
                    break;
            }
        }

        private NameValueCollection GetCookiesAsNameValueCollection()
        {
            var cookieCollection = new NameValueCollection();
            if (Cookies == null) return cookieCollection;

            foreach (string cookieName in Cookies.AllKeys)
            {
                var cookie = Cookies.Get(cookieName);
                if (cookie == null) continue;

                foreach (string cookieValue in cookie.Values)
                {
                    cookieCollection.Add(cookie.Name, cookieValue);
                }
            }

            return cookieCollection;
        }

        private NameValueCollection GetRouteDataAsNameValueCollection()
        {
            var routeDataCollection = new NameValueCollection();

            foreach (string routeKey in RouteValues.Keys)
            {
                object routeValue;
                
                if (RouteValues.TryGetValue(routeKey, out routeValue) && routeValue != null)
                {
                    routeDataCollection.Add(routeKey, routeValue.ToString());
                }
            }

            return routeDataCollection;
        }

        private void PopulateValues()
        {
            values = new NameValueCollection
                     {
                         Query,
                         GetRouteDataAsNameValueCollection(),
                         Form,
                         Headers,
                         Form,
                         GetCookiesAsNameValueCollection(),
                         ServerVariables
                     };
        }
    }
}
