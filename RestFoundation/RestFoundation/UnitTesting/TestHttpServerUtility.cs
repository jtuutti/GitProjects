// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Web;

namespace RestFoundation.UnitTesting
{
    internal sealed class TestHttpServerUtility : HttpServerUtilityBase
    {
        private readonly string m_virtualUrl;

        internal TestHttpServerUtility(string virtualUrl)
        {
            if (virtualUrl == null)
            {
                throw new ArgumentNullException("virtualUrl");
            }

            m_virtualUrl = virtualUrl.TrimStart('~', '/', ' ');
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
