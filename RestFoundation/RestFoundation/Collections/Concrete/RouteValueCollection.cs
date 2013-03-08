// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace RestFoundation.Collections.Concrete
{
    /// <summary>
    /// Represents an object value collection.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public class RouteValueCollection : IRouteValueCollection
    {
        private readonly IDictionary<string, object> m_collection;

        internal RouteValueCollection(IDictionary<string, object> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            m_collection = collection;
        }

        /// <summary>
        /// Gets a collection of all collection keys.
        /// </summary>
        public ICollection<string> Keys
        {
            get
            {
                return new ReadOnlyCollection<string>(new List<string>(m_collection.Keys));
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
        public IEnumerator<object> GetEnumerator()
        {
            return new ObjectValueEnumerator(m_collection.GetEnumerator());
        }

        /// <summary>
        /// Gets a value by the key in the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The corresponding value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the key is not a part of the collection.</exception>
        public object Get(string key)
        {
            object value = TryGet(key);

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
        public object TryGet(string key)
        {
            object value;
            return m_collection.TryGetValue(key, out value) ? value : null;
        }

        /// <summary>
        /// Returns the pairs of keys and objects as a dictionary.
        /// </summary>
        /// <returns>The dictionary of keys and objects.</returns>
        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>(m_collection, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            var collectionStringBuilder = new StringBuilder();

            foreach (KeyValuePair<string, object> item in m_collection)
            {
                if (collectionStringBuilder.Length > 0)
                {
                    collectionStringBuilder.Append("&");
                }

                collectionStringBuilder.AppendFormat("{0}={1}", item.Key, item.Value ?? String.Empty);
            }

            return collectionStringBuilder.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_collection.GetEnumerator();
        }

        private class ObjectValueEnumerator : IEnumerator<object>
        {
            private readonly IEnumerator m_enumerator;

            public ObjectValueEnumerator(IEnumerator enumerator)
            {
                m_enumerator = enumerator;
            }

            public object Current
            {
                get
                {
                    return m_enumerator.Current;
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
