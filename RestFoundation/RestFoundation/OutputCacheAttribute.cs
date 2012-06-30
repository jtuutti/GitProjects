using System;
using System.Web.UI;

namespace RestFoundation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class OutputCacheAttribute : Attribute
    {
        public OutputCacheAttribute()
        {
            CacheSettings = new OutputCacheParameters();
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
    }
}
