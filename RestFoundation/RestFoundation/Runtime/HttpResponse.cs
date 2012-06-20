using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using RestFoundation.Collections;
using RestFoundation.Collections.Concrete;

namespace RestFoundation.Runtime
{
    public class HttpResponse : IHttpResponse
    {
        private const string LineBreak = "<br/>";

        private static HttpContext Context
        {
            get
            {
                return HttpContext.Current;
            }
        }

        public Stream Output
        {
            get
            {
                return Context.Response.OutputStream;
            }
        }

        public TextWriter OutputWriter
        {
            get
            {
                return Context.Response.Output;
            }
        }

        public Stream OutputFilter
        {
            get
            {
                return Context.Response.Filter;
            }
            set
            {
                Context.Response.Filter = value;
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

        public void RemoveHeader(string headerName)
        {
            if (headerName == null) throw new ArgumentNullException("headerName");

            Context.Response.Headers.Remove(headerName);
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

        public void SetCharsetEncoding(Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");

            Context.Response.ContentEncoding = encoding;
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

        public void RemoveCookie(HttpCookie cookie)
        {
            if (cookie == null) throw new ArgumentNullException("cookie");

            cookie.Expires = DateTime.Now.AddDays(-1);
            Context.Response.SetCookie(cookie);
        }

        public object GetHttpItem(string name)
        {
            return Context.Items[name];
        }

        public void SetHttpItem(string name, object value)
        {
            Context.Items[name] = value;
        }

        public void RemoveHttpItem(string name)
        {
            Context.Items.Remove(name);
        }

        public void Flush()
        {
            Context.Response.Flush();
        }

        public void Clear()
        {
            Context.Response.Clear();
        }

        public void ClearHeaders()
        {
            Context.Response.ClearHeaders();
        }

        public void Redirect(string url)
        {
            Redirect(url, false, true);
        }

        public void Redirect(string url, bool isPermanent)
        {
            Redirect(url, isPermanent, true);
        }

        public void Redirect(string url, bool isPermanent, bool endResponse)
        {
            if (String.IsNullOrEmpty(url)) throw new ArgumentNullException("url");

            if (isPermanent)
            {
                Context.Response.RedirectPermanent(url, endResponse);
            }
            else
            {
                Context.Response.Redirect(url, endResponse);
            }
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

        public IHttpResponse Write(string value)
        {
            Context.Response.Write(value);
            return this;
        }

        public IHttpResponse Write(object obj)
        {
            Context.Response.Write(obj);
            return this;
        }

        public IHttpResponse WriteLine()
        {
            Context.Response.Write(LineBreak);
            return this;
        }

        public IHttpResponse WriteLine(string value)
        {
            Context.Response.Write(value);
            Context.Response.Write(LineBreak);
            return this;
        }

        public IHttpResponse WriteLine(byte times)
        {
            for (byte i = 0; i < times; i++)
            {
                WriteLine();
            }

            return this;
        }

        public IHttpResponse WriteFormat(string format, params object[] values)
        {
            if (format == null) throw new ArgumentNullException("format");

            Context.Response.Write(String.Format(format, values));
            return this;
        }

        public IHttpResponse WriteFormat(CultureInfo provider, string format, params object[] values)
        {
            if (format == null) throw new ArgumentNullException("format");

            Context.Response.Write(String.Format(provider, format, values));
            return this;
        }
    }
}
