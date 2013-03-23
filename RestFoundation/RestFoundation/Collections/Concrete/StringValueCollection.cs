// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

namespace RestFoundation.Collections.Concrete
{
    /// <summary>
    /// Represents a string value collection.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public class StringValueCollection : IStringValueCollection
    {
        private readonly NameValueCollection m_collection;

        internal StringValueCollection(NameValueCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            m_collection = collection;
        }

        /// <summary>
        /// Gets a read-only collection of all keys.
        /// </summary>
        public IReadOnlyCollection<string> Keys
        {
            get
            {
                return m_collection.AllKeys;
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
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<string> GetEnumerator()
        {
            return new StringValueEnumerator(m_collection.GetEnumerator());
        }

        /// <summary>
        /// Gets a value by the key in the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The corresponding value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the key is not a part of the collection.</exception>
        public string Get(string key)
        {
            string value = TryGet(key);

            if (value == null)
            {
                throw new ArgumentOutOfRangeException("key", Resources.Global.InvalidKey);
            }

            return value;
        }

        /// <summary>
        /// Gets a value by the key in the collection or null if the key is not a part of the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The corresponding value.</returns>
        public string TryGet(string key)
        {
            return m_collection.Get(key);
        }

        /// <summary>
        /// Returns all values associated with the key as a collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The list of values for the key.</returns>
        public IList<string> GetValues(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            return m_collection.GetValues(key) ?? new string[0];
        }

        /// <summary>
        /// Returns the pairs of keys and objects as a name-value collection.
        /// </summary>
        /// <returns>The name-value collection of keys and objects.</returns>
        public NameValueCollection ToNameValueCollection()
        {
            return new NameValueCollection(m_collection);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return m_collection.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_collection.GetEnumerator();
        }

        private class StringValueEnumerator : IEnumerator<string>
        {
            private readonly IEnumerator m_enumerator;

            public StringValueEnumerator(IEnumerator enumerator)
            {
                m_enumerator = enumerator;
            }

            public string Current
            {
                get
                {
                    return (string) m_enumerator.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public bool MoveNext()
            {
                return m_enumerator.MoveNext();
            }

            public void Reset()
            {
                m_enumerator.Reset();
            }

            public void Dispose()
            {
            }
        }
    }
}
