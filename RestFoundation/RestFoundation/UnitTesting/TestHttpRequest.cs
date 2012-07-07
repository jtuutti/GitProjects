using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;

namespace RestFoundation.UnitTesting
{
    internal sealed class TestHttpRequest : HttpRequestBase, IDisposable
    {
        private readonly string m_httpMethod;
        private readonly string m_executionFilePath;
        private readonly string m_path;
        private readonly string m_rawUrl;
        private readonly Uri m_url;
        private readonly HttpCookieCollection m_cookies;
        private readonly NameValueCollection m_form;
        private readonly NameValueCollection m_headers;
        private readonly NameValueCollection m_queryString;
        private readonly NameValueCollection m_serverVariables;
        private readonly NameValueCollection m_params;
        private readonly Stream m_body;
        private Stream m_filter;
        private bool m_isDisposed;

        internal TestHttpRequest(string relativeUrl, string httpMethod)
        {
            string[] urlParts = relativeUrl.Split(new[] { '?', '#' }, StringSplitOptions.RemoveEmptyEntries);

            m_httpMethod = httpMethod;
            m_executionFilePath = urlParts[0].Trim();
            m_path = m_rawUrl = relativeUrl.Trim().TrimStart('~');
            m_url = new Uri(new Uri("http://localhost"), m_path);
            m_cookies = new HttpCookieCollection();
            m_form = new NameValueCollection();
            m_headers = new NameValueCollection();
            m_queryString = new NameValueCollection();
            m_serverVariables = new NameValueCollection();
            m_params = new NameValueCollection();
            m_body = new MemoryStream();
            m_filter = new MemoryStream();
        }

        public override Encoding ContentEncoding { get; set; }
        public override string ContentType { get; set; }

        public override bool IsLocal
        {
            get
            {
                return true;
            }
        }

        public override bool IsSecureConnection
        {
            get
            {
                return false;
            }
        }

        public override bool IsAuthenticated
        {
            get
            {
                return true;
            }
        }

        public override Stream InputStream
        {
            get
            {
                return m_body;
            }
        }

        public override Stream Filter
        {
            get
            {
                return m_filter;
            }
            set
            {
                m_filter = value;
            }
        }

        public override string[] AcceptTypes
        {
            get
            {
                return new string[0];
            }
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get
            {
                return m_executionFilePath;
            }
        }

        public override string ApplicationPath
        {
            get
            {
                return m_path;
            }
        }

        public override string CurrentExecutionFilePath
        {
            get
            {
                return m_path;
            }
        }

        public override string HttpMethod
        {
            get
            {
                return m_httpMethod;
            }
        }

        public override string Path
        {
            get
            {
                return m_path;
            }
        }

        public override string PathInfo
        {
            get
            {
                return String.Empty;
            }
        }

        public override string RawUrl
        {
            get
            {
                return m_rawUrl;
            }
        }

        public override Uri Url
        {
            get
            {
                return m_url;
            }
        }

        public override HttpCookieCollection Cookies
        {
            get
            {
                return m_cookies;
            }
        }

        public override NameValueCollection Form
        {
            get
            {
                return m_form;
            }
        }

        public override NameValueCollection Headers
        {
            get
            {
                return m_headers;
            }
        }

        public override NameValueCollection QueryString
        {
            get
            {
                return m_queryString;
            }
        }

        public override NameValueCollection ServerVariables
        {
            get
            {
                return m_serverVariables;
            }
        }

        public override NameValueCollection Params
        {
            get
            {
                return m_params;
            }
        }

        public void Dispose()
        {
            if (m_isDisposed)
            {
                return;
            }

            m_body.Dispose();
            m_filter.Dispose();
            m_isDisposed = true;
        }
    }
}
