using System;
using System.Reflection;
using System.Web;
using System.Web.UI;
using RestFoundation.Runtime;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents an output cache behavior for a service or a service method.
    /// </summary>
    public class OutputCacheBehavior : ServiceBehavior
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputCacheBehavior"/> class.
        /// </summary>
        public OutputCacheBehavior()
        {
            CacheSettings = new OutputCacheParameters();
            VaryByParam = "none";
        }

        internal OutputCacheParameters CacheSettings { get; private set; }

        /// <summary>
        /// Gets or sets the amount of time that a cache entry is to remain in the output cache.
        /// </summary>
        public TimeSpan DurationInSeconds
        {
            get
            {
                return TimeSpan.FromSeconds(CacheSettings.Duration);
            }
            set
            {
                CacheSettings.Duration = Convert.ToInt32(value.TotalSeconds);
            }
        }

        /// <summary>
        /// Gets or sets a value that determines the location of the cache entry.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value that determines whether HTTP cache-control: no-store directive is set.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a comma delimited set of character sets (content encodings) used to vary the cache entry.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a comma delimited set of custom strings used to vary the cache entry.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a comma delimited set of HTTP header names associated with the request used to vary the cache entry.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a comma delimited set of query string or form POST parameters used to vary the cache entry.
        /// </summary>
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

        /// <summary>
        /// Called after a service method is executed.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="service">The service object.</param>
        /// <param name="method">The service method.</param>
        /// <param name="returnedObj">The service method returned object.</param>
        public override void OnMethodExecuted(IServiceContext context, object service, MethodInfo method, object returnedObj)
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
