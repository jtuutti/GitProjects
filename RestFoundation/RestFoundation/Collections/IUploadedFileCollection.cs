// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;

namespace RestFoundation.Collections
{
    /// <summary>
    /// Defines an uploaded file collection.
    /// </summary>
    public interface IUploadedFileCollection : IEnumerable<IUploadedFile>
    {
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
        IUploadedFile Get(string key);

        /// <summary>
        /// Gets a value by the key in the collection or null if the key is not a part of the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The corresponding value.</returns>
        IUploadedFile TryGet(string key);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through
        /// the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        new IEnumerator<IUploadedFile> GetEnumerator();
    }
}
