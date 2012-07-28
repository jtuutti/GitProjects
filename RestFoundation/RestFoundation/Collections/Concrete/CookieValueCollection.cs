using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Web;

namespace RestFoundation.Collections.Concrete
{
    /// <summary>
    /// Represents a cookie collection.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public class CookieValueCollection : ICookieValueCollection
    {
        private readonly HttpCookieCollection m_values;

        internal CookieValueCollection(HttpCookieCollection values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            m_values = values;
        }

        /// <summary>
        /// Gets a collection of all collection keys.
        /// </summary>
        public ICollection<string> Keys
        {
            get
            {
                return new ReadOnlyCollection<string>(m_values.AllKeys);
            }
        }

        /// <summary>
        /// Gets a value containing the count of all collection items.
        /// </summary>
        public int Count
        {
            get
            {
                return m_values.Count;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<HttpCookie> GetEnumerator()
        {
            return new CookieValueEnumerator(m_values.GetEnumerator());
        }

        /// <summary>
        /// Gets a value by the key in the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The corresponding value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the key is not a part of the collection.</exception>
        public HttpCookie Get(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            HttpCookie value = TryGet(key);

            if (value == null)
            {
                throw new ArgumentOutOfRangeException("key", "Invalid key provided");
            }

            return value;
        }

        /// <summary>
        /// Gets a value by the key in the collection or null if the key is not a part of the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The corresponding value.</returns>
        public HttpCookie TryGet(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            return m_values.Get(key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_values.GetEnumerator();
        }

        private class CookieValueEnumerator : IEnumerator<HttpCookie>
        {
            private readonly IEnumerator m_enumerator;

            public CookieValueEnumerator(IEnumerator enumerator)
            {
                m_enumerator = enumerator;
            }

            public HttpCookie Current
            {
                get
                {
                    return (HttpCookie) m_enumerator.Current;
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
