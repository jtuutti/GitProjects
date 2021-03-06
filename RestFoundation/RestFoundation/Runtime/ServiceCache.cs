﻿// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents the default cache implementation that uses ASP .NET cache.
    /// </summary>
    public class ServiceCache : IServiceCache
    {
        private readonly Cache m_cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCache"/> class.
        /// </summary>
        public ServiceCache()
        {
            m_cache = HttpRuntime.Cache; // works in and out of the ASP.NET process

            if (m_cache == null)
            {
                throw new InvalidOperationException(Resources.Global.UnableToInitializeCache);
            }
        }

        /// <summary>
        /// Gets a cached value by its key.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>The corresponding value.</returns>
        public object this[string key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Update(key, value);
            }
        }

        /// <summary>
        /// Returns a value indicating whether the provided key is found in the cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>true if the key was found; otherwise, false.</returns>
        public virtual bool Contains(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            return m_cache.Get(key) != null;
        }

        /// <summary>
        /// Gets a cached value by its key.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>The corresponding value.</returns>
        public virtual object Get(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            return m_cache.Get(key);
        }

        /// <summary>
        /// Adds a key/value pair to the cache with the normal cache item priority.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The cached value.</param>
        public void Add(string key, object value)
        {
            Add(key, value, Cache.NoAbsoluteExpiration, CachePriority.Normal);
        }

        /// <summary>
        /// Adds a key/value pair to the cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The cached value.</param>
        /// <param name="priority">The cached item priority.</param>
        public void Add(string key, object value, CachePriority priority)
        {
            Add(key, value, Cache.NoAbsoluteExpiration, priority);
        }

        /// <summary>
        /// Adds a key/value pair to the cache with the normal cache item priority.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The cached value.</param>
        /// <param name="absoluteExpiration">A <see cref="DateTime"/> representing an absolute expiration time.</param>
        public void Add(string key, object value, DateTime absoluteExpiration)
        {
            Add(key, value, absoluteExpiration, CachePriority.Normal);
        }

        /// <summary>
        /// Adds a key/value pair to the cache with the normal cache item priority.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The cached value.</param>
        /// <param name="slidingExpiration">A <see cref="TimeSpan"/> representing a sliding expiration time.</param>
        public void Add(string key, object value, TimeSpan slidingExpiration)
        {
            Add(key, value, slidingExpiration, CachePriority.Normal);
        }

        /// <summary>
        /// Adds a key/value pair to the cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The cached value.</param>
        /// <param name="absoluteExpiration">A <see cref="DateTime"/> representing an absolute expiration time.</param>
        /// <param name="priority">The cached item priority.</param>
        public virtual void Add(string key, object value, DateTime absoluteExpiration, CachePriority priority)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            m_cache.Add(key, value, null, absoluteExpiration, Cache.NoSlidingExpiration, ConvertCachePriority(priority), null);
        }

        /// <summary>
        /// Adds a key/value pair to the cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The cached value.</param>
        /// <param name="slidingExpiration">A <see cref="TimeSpan"/> representing a sliding expiration time.</param>
        /// <param name="priority">The cached item priority.</param>
        public virtual void Add(string key, object value, TimeSpan slidingExpiration, CachePriority priority)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            m_cache.Add(key, value, null, Cache.NoAbsoluteExpiration, slidingExpiration, ConvertCachePriority(priority), null);
        }

        /// <summary>
        /// Updated the cached value for the corresponding cache key.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The cached value.</param>
        public virtual void Update(string key, object value)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            m_cache[key] = value;
        }

        /// <summary>
        /// Removes a cache entry by its key.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>If the cache key was found and the entry was removed.</returns>
        public virtual bool Remove(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            return m_cache.Remove(key) != null;
        }

        /// <summary>
        /// Clears all entries in the cache.
        /// </summary>
        public virtual void Clear()
        {
            IDictionaryEnumerator enumerator = m_cache.GetEnumerator();

            var keys = new HashSet<string>();

            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key.ToString());
            }

            foreach (string key in keys)
            {
                m_cache.Remove(key);
            }
        }

        private static CacheItemPriority ConvertCachePriority(CachePriority priority)
        {
            CacheItemPriority cachedItemPriority;

            switch (priority)
            {
                case CachePriority.Low:
                    cachedItemPriority = CacheItemPriority.Low;
                    break;
                case CachePriority.High:
                    cachedItemPriority = CacheItemPriority.High;
                    break;
                default:
                    cachedItemPriority = CacheItemPriority.Normal;
                    break;
            }

            return cachedItemPriority;
        }
    }
}
