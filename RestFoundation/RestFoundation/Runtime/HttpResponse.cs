using System;
using System.Net;
using System.Text;
using System.Web;
using RestFoundation.Collections;
using RestFoundation.Collections.Concrete;

namespace RestFoundation.Runtime
{
    public class HttpResponse : IHttpResponse
    {
        private readonly IHttpResponseOutput m_output;

        public HttpResponse()
        {
            m_output = new HttpResponseOutput();
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

        public IHttpResponseOutput Output
        {
            get
            {
                return m_output;
            }
        }

        public bool TrySkipIisCustomErrors
        {
            get
            {
                return Context.Response.TrySkipIisCustomErrors;
            }
            set
            {
                Context.Response.TrySkipIisCustomErrors = value;
            }
        }

        public string GetHeader(string headerName)
        {
            if (headerName == null) throw new ArgumentNullException("headerName");

            return Context.Response.Headers.Get(headerName);
        }

        public void SetHeader(string headerName, string headerValue)
        {
            if (headerName == null) throw new ArgumentNullException("headerName");
            if (headerValue == null) throw new ArgumentNullException("headerValue");

            if (Context.Response.Headers[headerName] != null)
            {
                Context.Response.Headers.Remove(headerName);
            }

            Context.Response.AppendHeader(headerName, headerValue);
        }

        public bool RemoveHeader(string headerName)
        {
            if (headerName == null) throw new ArgumentNullException("headerName");

            if (Array.IndexOf(Context.Response.Headers.AllKeys, headerName) < 0)
            {
                return false;
            }

            Context.Response.Headers.Remove(headerName);
            return true;
        }

        public void ClearHeaders()
        {
            Context.Response.ClearHeaders();
        }

        public void SetCharsetEncoding(Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");

            Context.Response.ContentEncoding = encoding;
        }

        public HttpStatusCode GetStatusCode()
        {
            return (HttpStatusCode) Context.Response.StatusCode;
        }

        public string GetStatusDescription()
        {
            return Context.Response.StatusDescription;
        }

        public void SetStatus(HttpStatusCode statusCode)
        {
            SetStatus(statusCode, String.Empty);
        }

        public void SetStatus(HttpStatusCode statusCode, string statusDescription)
        {
            Context.Response.StatusCode = (int) statusCode;
            Context.Response.StatusDescription = statusDescription ?? String.Empty;
        }

        public HttpCookie GetCookie(string cookieName)
        {
            if (cookieName == null) throw new ArgumentNullException("cookieName");

            return Context.Response.Cookies.Get(cookieName);
        }

        public ICookieValueCollection GetCookies()
        {
            return new CookieValueCollection(Context.Response.Cookies);
        }

        public void SetCookie(HttpCookie cookie)
        {
            if (cookie == null) throw new ArgumentNullException("cookie");

            Context.Response.SetCookie(cookie);
        }

        public void ExpireCookie(HttpCookie cookie)
        {
            if (cookie == null) throw new ArgumentNullException("cookie");

            cookie.Expires = DateTime.Now.AddDays(-1);
            Context.Response.SetCookie(cookie);
        }

        public string MapPath(string filePath)
        {
            return Context.Server.MapPath(filePath);
        }

        public void SetFileDependencies(string filePath)
        {
            SetFileDependencies(filePath, HttpCacheability.ServerAndPrivate);
        }

        public void SetFileDependencies(string filePath, HttpCacheability cacheability)
        {
            if (String.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");

            Context.Response.AddFileDependency(filePath);
            Context.Response.Cache.SetCacheability(cacheability);
            Context.Response.Cache.SetETagFromFileDependencies();
            Context.Response.Cache.SetLastModifiedFromFileDependencies();
            Context.Response.Cache.VaryByParams["*"] = true;

            foreach (string headerName in Context.Request.Headers.AllKeys)
            {
                if (headerName.StartsWith("Accept", StringComparison.OrdinalIgnoreCase) ||
                    headerName.StartsWith("X-", StringComparison.OrdinalIgnoreCase))
                {
                    Context.Response.Cache.VaryByHeaders[headerName] = true;
                }
            }
        }

        public void TransmitFile(string filePath)
        {
            Context.Response.TransmitFile(filePath);
        }
    }
}
