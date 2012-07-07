using System;
using System.Collections;
using System.Security.Principal;
using System.Web;

namespace RestFoundation.UnitTesting
{
    internal sealed class TestHttpContext : HttpContextBase, IDisposable
    {
        private readonly TestHttpRequest m_request;
        private readonly TestHttpResponse m_response;
        private readonly TestHttpServerUtility m_server;
        private readonly IDictionary m_items;
        private IPrincipal m_user;

        internal TestHttpContext(string relativeUrl, string httpMethod)
        {
            m_request = new TestHttpRequest(relativeUrl, httpMethod);
            m_response = new TestHttpResponse();
            m_server = new TestHttpServerUtility(relativeUrl);
            m_items = new Hashtable();
            m_user = new GenericPrincipal(new GenericIdentity("Test"), new[] { "Tester" });
        }

        public override HttpRequestBase Request
        {
            get
            {
                return m_request;
            }
        }

        public override HttpResponseBase Response
        {
            get
            {
                return m_response;
            }
        }

        public override HttpServerUtilityBase Server
        {
            get
            {
                return m_server;
            }
        }

        public override IDictionary Items
        {
            get
            {
                return m_items;
            }
        }

        public override IPrincipal User
        {
            get
            {
                return m_user;
            }
            set
            {
                m_user = value;
            }
        }

        public override void ClearError()
        {
        }

        public void Dispose()
        {
            m_request.Dispose();
            m_response.Dispose();
        }
    }
}
