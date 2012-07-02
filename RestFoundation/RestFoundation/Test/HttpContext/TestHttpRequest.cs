using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace RestFoundation.Test.HttpContext
{
    public sealed class TestHttpRequest : HttpRequestBase
    {
        private readonly string m_executionFilePath;
        private readonly string m_rawUrl;
        private readonly string m_httpMethod;
        private readonly HttpCookieCollection m_cookies;
        private readonly NameValueCollection m_form;
        private readonly NameValueCollection m_headers;
        private readonly NameValueCollection m_queryString;
        private readonly NameValueCollection m_serverVariables;
        private readonly NameValueCollection m_params;

        internal TestHttpRequest(string relativeUrl, string httpMethod)
        {
            string[] urlParts = relativeUrl.Split(new[] { '?', '#' }, StringSplitOptions.RemoveEmptyEntries);

            m_executionFilePath = urlParts[0].Trim();
            m_rawUrl = relativeUrl.Trim().TrimStart('~');
            m_httpMethod = httpMethod;
            m_cookies = new HttpCookieCollection();
            m_form = new NameValueCollection();
            m_headers = new NameValueCollection();
            m_queryString = new NameValueCollection();
            m_serverVariables = new NameValueCollection();
            m_params = new NameValueCollection();
        }

        public override Encoding ContentEncoding { get; set; }
        public override string ContentType { get; set; }

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

        public override string HttpMethod
        {
            get
            {
                return m_httpMethod;
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
    }
}
