// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;

namespace RestFoundation
{
    /// <summary>
    /// Defines a cache abstraction.
    /// </summary>
    public interface IServiceCache
    {
        /// <summary>
        /// Gets a cached value by its key.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>The corresponding value.</returns>
        object this[string key] { get; set; }

        /// <summary>
        /// Returns a value indicating whether the provided key is found in the cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>true if the key was found; otherwise, false.</returns>
        bool Contains(string key);

        /// <summary>
        /// Gets a cached value by its key.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>The corresponding value.</returns>
        object Get(string key);
        
        /// <summary>
        /// Adds a key/value pair to the cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The cached value.</param>
        void Add(string key, object value);

        /// <summary>
        /// Adds a key/value pair to the cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The cached value.</param>
        /// <param name="absoluteExpiration">A <see cref="DateTime"/> representing an absolute expiration time.</param>
        void Add(string key, object value, DateTime absoluteExpiration);

        /// <summary>
        /// Adds a key/value pair to the cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The cached value.</param>
        /// <param name="slidingExpiration">A <see cref="TimeSpan"/> representing a sliding expiration time.</param>
        void Add(string key, object value, TimeSpan slidingExpiration);

        /// <summary>
        /// Updated the cached value for the corresponding cache key.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The cached value.</param>
        void Update(string key, object value);

        /// <summary>
        /// Removes a cache entry by its key.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>If the cache key was found and the entry was removed.</returns>
        bool Remove(string key);

        /// <summary>
        /// Clears all entries in the cache.
        /// </summary>
        void Clear();
    }
}
