// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace RestFoundation.Collections
{
    /// <summary>
    /// Defines an string value collection.
    /// </summary>
    public interface IStringValueCollection : IEnumerable<string>
    {
        /// <summary>
        /// Gets a read-only collection of all keys.
        /// </summary>
        IReadOnlyCollection<string> Keys { get; }

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
        string Get(string key);

        /// <summary>
        /// Gets a value by the key in the collection or null if the key is not a part of the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The corresponding value.</returns>
        string TryGet(string key);

        /// <summary>
        /// Returns all values associated with the key as a collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The list of values for the key.</returns>
        IList<string> GetValues(string key);

        /// <summary>
        /// Returns the pairs of keys and objects as a name-value collection.
        /// </summary>
        /// <returns>The name-value collection of keys and objects.</returns>
        NameValueCollection ToNameValueCollection();
    }
}
