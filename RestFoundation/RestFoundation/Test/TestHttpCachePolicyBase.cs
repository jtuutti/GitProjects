using System.Web;

namespace RestFoundation.Test
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

        public override void SetCacheability(HttpCacheability cacheability)
        {
        }

        public override void SetETagFromFileDependencies()
        {
        }

        public override void SetExpires(System.DateTime date)
        {
        }

        public override void SetLastModifiedFromFileDependencies()
        {
        }

        public override void SetMaxAge(System.TimeSpan delta)
        {
        }
    }
}
