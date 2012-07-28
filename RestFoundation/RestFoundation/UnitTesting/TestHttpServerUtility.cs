using System;
using System.Web;

namespace RestFoundation.UnitTesting
{
    internal sealed class TestHttpServerUtility : HttpServerUtilityBase
    {
        private readonly string m_relativeUrl;
        private int m_scriptTimeout;

        internal TestHttpServerUtility(string relativeUrl)
        {
            if (relativeUrl == null)
            {
                throw new ArgumentNullException("relativeUrl");
            }

            m_relativeUrl = relativeUrl.TrimStart('~', '/', ' ');
            m_scriptTimeout = 60;
        }

        public override int ScriptTimeout
        {
            get
            {
                return m_scriptTimeout;
            }
            set
            {
                m_scriptTimeout = value;
            }
        }

        public override string MapPath(string path)
        {
            if (path == null)
            {
                return null;
            }

            return path.ToLowerInvariant().Replace("http://localhost/" + m_relativeUrl, Environment.CurrentDirectory).Replace("/", @"\").TrimStart('~', '\\');
        }
    }
}
