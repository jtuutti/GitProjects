using System;
using System.Web;

namespace RestFoundation.Test
{
    internal sealed class TestHttpRequest : HttpRequestBase
    {
        private readonly string m_relativeUrl;

        public TestHttpRequest(string relativeUrl)
        {
            m_relativeUrl = relativeUrl;
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get
            {
                return m_relativeUrl;
            }
        }

        public override string PathInfo
        {
            get
            {
                return String.Empty;
            }
        }
    }
}
