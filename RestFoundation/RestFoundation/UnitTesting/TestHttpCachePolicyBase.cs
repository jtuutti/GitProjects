// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>

using System;
using System.Web;

namespace RestFoundation.UnitTesting
{
    internal sealed class TestHttpCachePolicyBase : HttpCachePolicyBase
    {
        public override HttpCacheVaryByHeaders VaryByHeaders
        {
            get
            {
                return new HttpCacheVaryByHeaders();
            }
        }

        public override HttpCacheVaryByParams VaryByParams
        {
            get
            {
                return new HttpCacheVaryByParams();
            }
        }

        public override void AddValidationCallback(HttpCacheValidateHandler handler, object data)
        {
        }

        public override void SetCacheability(HttpCacheability cacheability)
        {
        }

        public override void SetETagFromFileDependencies()
        {
        }

        public override void SetExpires(DateTime date)
        {
        }

        public override void SetLastModifiedFromFileDependencies()
        {
        }

        public override void SetMaxAge(TimeSpan delta)
        {
        }

        public override void SetProxyMaxAge(TimeSpan delta)
        {
        }
    }
}
