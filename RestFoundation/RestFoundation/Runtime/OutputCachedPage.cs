// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Web.UI;

namespace RestFoundation.Runtime
{
    internal sealed class OutputCachedPage : Page
    {
        private readonly OutputCacheParameters m_cacheSettings;

        public OutputCachedPage(OutputCacheParameters cacheSettings)
        {
            if (cacheSettings == null)
            {
                throw new ArgumentNullException("cacheSettings");
            }

            ID = Guid.NewGuid().ToString();
            m_cacheSettings = cacheSettings;
        }

        protected override void FrameworkInitialize()
        {
            base.FrameworkInitialize();
            InitOutputCache(m_cacheSettings);
        }
    }
}
