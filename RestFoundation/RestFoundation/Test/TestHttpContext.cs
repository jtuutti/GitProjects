using System.Web;

namespace RestFoundation.Test
{
    internal sealed class TestHttpContext : HttpContextBase
    {
        private readonly HttpRequestBase m_request;

        public TestHttpContext(string relativeUrl, string httpMethod)
        {
            m_request = new TestHttpRequest(relativeUrl, httpMethod);
        }

        public override HttpRequestBase Request
        {
            get
            {
                return m_request;
            }
        }
    }
}
