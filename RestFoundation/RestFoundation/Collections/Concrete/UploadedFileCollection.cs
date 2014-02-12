// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using RestFoundation.Runtime;

namespace RestFoundation.Collections.Concrete
{
    /// <summary>
    /// Represents an uploaded files collection.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public class UploadedFileCollection : IUploadedFileCollection
    {
        private readonly ICollection<IUploadedFile> m_collection;

        internal UploadedFileCollection(HttpFileCollectionBase collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            m_collection = new List<IUploadedFile>();

            PopulateFiles(collection);
        }

        /// <summary>
        /// Gets a list of all keys.
        /// </summary>
        public IReadOnlyList<string> Keys
        {
            get
            {
                return m_collection.Select(f => f.Name).ToArray();
            }
        }

        /// <summary>
        /// Gets a value containing the count of all collection items.
        /// </summary>
        public int Count
        {
            get
            {
                return m_collection.Count;
            }
        }

        /// <summary>
        /// Gets a value by the key in the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The corresponding value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the key is not a part of the collection.</exception>
        public IUploadedFile Get(string key)
        {
            IUploadedFile file = TryGet(key);

            if (file == null)
            {
                throw new ArgumentOutOfRangeException("key", Resources.Global.InvalidKey);
            }

            return file;
        }

        /// <summary>
        /// Gets a value by the key in the collection or null if the key is not a part of the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The corresponding value.</returns>
        public IUploadedFile TryGet(string key)
        {
            return m_collection.FirstOrDefault(f => String.Equals(key, f.Name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through
        /// the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<IUploadedFile> GetEnumerator()
        {
            return m_collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void PopulateFiles(HttpFileCollectionBase collection)
        {
            foreach (string fileName in collection.AllKeys)
            {
                var postedFile = collection.Get(fileName);

                if (postedFile != null)
                {
                    m_collection.Add(new UploadedFile(postedFile));
                }
            }
        }
    }
}
