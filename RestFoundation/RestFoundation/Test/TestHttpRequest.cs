using System;
using System.Web;

namespace RestFoundation.Test
{
    internal sealed class TestHttpRequest : HttpRequestBase
    {
        private readonly string m_executionFilePath;
        private readonly string m_rawUrl;

        public TestHttpRequest(string relativeUrl)
        {
            string[] urlParts = relativeUrl.Split(new[] { '?', '#' }, StringSplitOptions.RemoveEmptyEntries);

            m_executionFilePath = urlParts[0].Trim();
            m_rawUrl = relativeUrl.Trim().TrimStart('~');
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get
            {
                return m_executionFilePath;
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
    }
}
