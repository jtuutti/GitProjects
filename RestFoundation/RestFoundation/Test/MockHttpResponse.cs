using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace RestFoundation.Test
{
    public class MockHttpResponse : IHttpResponse
    {
        private readonly NameValueCollection m_headers;
        private readonly HttpCookieCollection m_cookies;
        private HttpStatusCode m_statusCode;
        private string m_statusDescription;

        public MockHttpResponse()
        {
            m_headers = new NameValueCollection();
            m_cookies = new HttpCookieCollection();

            m_statusCode = HttpStatusCode.OK;
            m_statusDescription = "OK";

            Output = new MockHttpResponseOutput();
        }

        public virtual IHttpResponseOutput Output { get; set; }
        public virtual bool TrySkipIisCustomErrors { get; set; }

        public virtual string GetHeader(string headerName)
        {
            if (headerName == null) throw new ArgumentNullException("headerName");

            return m_headers.Get(headerName);
        }

        public virtual void SetHeader(string headerName, string headerValue)
        {
            if (headerName == null) throw new ArgumentNullException("headerName");

            m_headers.Set(headerName, headerValue ?? String.Empty);
        }

        public virtual bool RemoveHeader(string headerName)
        {
            if (headerName == null) throw new ArgumentNullException("headerName");

            if (Array.IndexOf(m_headers.AllKeys, headerName) < 0)
            {
                return false;
            }

            m_headers.Remove(headerName);
            return true;
        }

        public virtual void ClearHeaders()
        {
            m_headers.Clear();
        }

        public virtual void SetCharsetEncoding(Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");

            Console.OutputEncoding = encoding;
        }

        public virtual HttpStatusCode GetStatusCode()
        {
            return m_statusCode;
        }

        public virtual string GetStatusDescription()
        {
            return m_statusDescription;
        }

        public virtual void SetStatus(HttpStatusCode statusCode)
        {
            SetStatus(statusCode, null);
        }

        public virtual void SetStatus(HttpStatusCode statusCode, string statusDescription)
        {
            m_statusCode = statusCode;
            m_statusDescription = statusDescription;
        }

        public virtual HttpCookie GetCookie(string cookieName)
        {
            if (cookieName == null) throw new ArgumentNullException("cookieName");

            return m_cookies.Get(cookieName);
        }

        public virtual void SetCookie(HttpCookie cookie)
        {
            if (cookie == null) throw new ArgumentNullException("cookie");

            m_cookies.Set(cookie);
        }

        public virtual void ExpireCookie(HttpCookie cookie)
        {
            if (cookie == null) throw new ArgumentNullException("cookie");

            m_cookies.Remove(cookie.Name);
        }

        public virtual void SetFileDependencies(string filePath)
        {
        }

        public virtual void SetFileDependencies(string filePath, HttpCacheability cacheability)
        {
        }

        public virtual void TransmitFile(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException("filePath");

            using (var reader = new StreamReader(filePath))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }
    }
}
