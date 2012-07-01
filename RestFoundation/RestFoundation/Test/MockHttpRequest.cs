using System;
using System.IO;
using System.Net;
using RestFoundation.Collections;
using RestFoundation.Collections.Concrete;
using RestFoundation.Collections.Specialized;
using RestFoundation.Runtime;

namespace RestFoundation.Test
{
    public class MockHttpRequest : IHttpRequest
    {
        public MockHttpRequest(string relativeServiceUrl)
        {
            if (String.IsNullOrEmpty(relativeServiceUrl)) throw new ArgumentNullException("relativeServiceUrl");

            IsLocal = true;
            Method = HttpMethod.Get;
            Url = new ServiceUri("http://localhost", relativeServiceUrl.TrimStart('~').Trim('/'));
            Body = new MemoryStream();

            QueryBag = new DynamicStringCollection();
            RouteValues = new ObjectValueCollection();
            Headers = new HeaderCollection();
            QueryString = new StringValueCollection();
            ServerVariables = new StringValueCollection();
            Cookies = new CookieValueCollection();
        }

        public virtual bool IsAjax { get; set; }
        public virtual bool IsSecure { get; set; }
        public virtual bool IsLocal { get; set; }
        public virtual HttpMethod Method { get; set; }
        public virtual ServiceUri Url { get; set; }
        public virtual Stream Body { get; set; }
        public virtual NetworkCredential Credentials { get; set; }

        public virtual dynamic QueryBag { get; set; }
        public virtual IObjectValueCollection RouteValues { get; set; }
        public virtual IHeaderCollection Headers { get; set; }
        public virtual IStringValueCollection QueryString { get; set; }
        public virtual IStringValueCollection ServerVariables { get; set; }
        public virtual ICookieValueCollection Cookies { get; set; }
    }
}
