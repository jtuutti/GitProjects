using System;
using System.Collections.Generic;
using System.Web;

namespace RestFoundation.Collections
{
    /// <summary>
    /// Defines a cookie collection.
    /// </summary>
    public interface ICookieValueCollection : IEnumerable<HttpCookie>
    {
        /// <summary>
        /// Gets a collection of all collection keys.
        /// </summary>
        ICollection<string> Keys { get; }

        /// <summary>
        /// Gets a value containing the count of all collection items.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets a value by the key in the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The corresponding value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the key is not a part of the collection.</exception>
        HttpCookie Get(string key);

        /// <summary>
        /// Gets a value by the key in the collection or null if the key is not a part of the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The corresponding value.</returns>
        HttpCookie TryGet(string key);
    }
}
