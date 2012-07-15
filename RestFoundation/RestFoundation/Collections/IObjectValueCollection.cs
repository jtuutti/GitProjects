using System;
using System.Collections.Generic;

namespace RestFoundation.Collections
{
    /// <summary>
    /// Defines an object value collection.
    /// </summary>
    public interface IObjectValueCollection : IEnumerable<object>
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
        object Get(string key);

        /// <summary>
        /// Gets a value by the key in the collection or null if the key is not a part of the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The corresponding value.</returns>
        object TryGet(string key);

        /// <summary>
        /// Returns the pairs of keys and objects as a dictionary.
        /// </summary>
        /// <returns>The dictionary of keys and objects.</returns>
        IDictionary<string, object> ToDictionary();
    }
}
