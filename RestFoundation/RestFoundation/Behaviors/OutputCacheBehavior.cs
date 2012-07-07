using System;
using System.Reflection;
using System.Web;
using System.Web.UI;
using RestFoundation.Runtime;

namespace RestFoundation.Behaviors
{
    public class OutputCacheBehavior : ServiceBehavior
    {
        public OutputCacheBehavior()
        {
            CacheSettings = new OutputCacheParameters();
            VaryByParam = "none";
        }

        internal OutputCacheParameters CacheSettings { get; private set; }

        public int DurationInSeconds
        {
            get
            {
                return CacheSettings.Duration;
            }
            set
            {
                CacheSettings.Duration = value;
            }
        }

        public OutputCacheLocation Location
        {
            get
            {
                return CacheSettings.Location;
            }
            set
            {
                CacheSettings.Location = value;
            }
        }

        public bool NoStore
        {
            get
            {
                return CacheSettings.NoStore;
            }
            set
            {
                CacheSettings.NoStore = value;
            }
        }

        public string VaryByContentEncoding
        {
            get
            {
                return (CacheSettings.VaryByContentEncoding ?? string.Empty);
            }
            set
            {
                CacheSettings.VaryByContentEncoding = value;
            }
        }

        public string VaryByCustom
        {
            get
            {
                return (CacheSettings.VaryByCustom ?? string.Empty);
            }
            set
            {
                CacheSettings.VaryByCustom = value;
            }
        }

        public string VaryByHeader
        {
            get
            {
                return (CacheSettings.VaryByHeader ?? string.Empty);
            }
            set
            {
                CacheSettings.VaryByHeader = value;
            }
        }

        public string VaryByParam
        {
            get
            {
                return (CacheSettings.VaryByParam ?? string.Empty);
            }
            set
            {
                CacheSettings.VaryByParam = value;
            }
        }

        public override void OnMethodExecuted(IServiceContext context, object service, MethodInfo method, object result)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (context.Request.Method != HttpMethod.Get && context.Request.Method != HttpMethod.Head)
            {
                return;
            }

            HttpContext httpContext = HttpContext.Current;

            if (httpContext == null)
            {
                return;
            }

            using (var page = new OutputCachedPage(CacheSettings))
            {
                page.ProcessRequest(httpContext);
            }
        }
    }
}
