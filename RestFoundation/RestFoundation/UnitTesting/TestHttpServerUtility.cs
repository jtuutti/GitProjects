// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Web;

namespace RestFoundation.UnitTesting
{
    internal sealed class TestHttpServerUtility : HttpServerUtilityBase
    {
        private readonly string m_virtualUrl;
        private int m_scriptTimeout;

        internal TestHttpServerUtility(string virtualUrl)
        {
            if (virtualUrl == null)
            {
                throw new ArgumentNullException("virtualUrl");
            }

            m_virtualUrl = virtualUrl.TrimStart('~', '/', ' ');
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

            return path.ToLowerInvariant().Replace("http://localhost/" + m_virtualUrl, Environment.CurrentDirectory).Replace("/", @"\").TrimStart('~', '\\');
        }
    }
}
