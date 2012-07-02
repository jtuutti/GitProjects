using System.Web;

namespace RestFoundation.Test.HttpContext
{
    public sealed class TestHttpContext : HttpContextBase
    {
        private readonly HttpRequestBase m_request;
        private readonly HttpResponseBase m_response;

        internal TestHttpContext(string relativeUrl, string httpMethod)
        {
            m_request = new TestHttpRequest(relativeUrl, httpMethod);
            m_response = new TestHttpResponse();
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
    }
}
