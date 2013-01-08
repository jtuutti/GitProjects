// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Web;
using System.Web.UI;
using RestFoundation.Runtime;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents a service method with a specific HTTP caching behavior.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class OutputCacheAttribute : ServiceMethodBehaviorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputCacheAttribute"/> class.
        /// </summary>
        public OutputCacheAttribute()
        {
            CacheSettings = new OutputCacheParameters();
            VaryByParam = "none";
        }

        /// <summary>
        /// Gets or sets the amount of time that a cache entry is to remain in the output cache.
        /// </summary>
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
        /// Gets or sets a value indicating whether HTTP cache-control: no-store directive is set.
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
                return CacheSettings.VaryByContentEncoding ?? String.Empty;
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
                return CacheSettings.VaryByCustom ?? String.Empty;
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
                return CacheSettings.VaryByHeader ?? String.Empty;
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
                return CacheSettings.VaryByParam ?? String.Empty;
            }
            set
            {
                CacheSettings.VaryByParam = value;
            }
        }

        internal OutputCacheParameters CacheSettings { get; private set; }

        /// <summary>
        /// Called after a service method is executed.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="behaviorContext">The "method executed" behavior context.</param>
        public override void OnMethodExecuted(IServiceContext serviceContext, MethodExecutedContext behaviorContext)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException("serviceContext");
            }

            if (serviceContext.Request.Method != HttpMethod.Get && serviceContext.Request.Method != HttpMethod.Head)
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
