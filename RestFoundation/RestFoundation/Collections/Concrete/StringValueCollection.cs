using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly NameValueCollection m_values;

        internal StringValueCollection(NameValueCollection values)
        {
            if (values == null) throw new ArgumentNullException("values");

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
        public IEnumerator<string> GetEnumerator()
        {
            return new StringValueEnumerator(m_values.GetEnumerator());
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
            if (value == null) throw new ArgumentOutOfRangeException("key", "Invalid key provided");

            return value;
        }

        /// <summary>
        /// Gets a value by the key in the collection or null if the key is not a part of the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The corresponding value.</returns>
        public string TryGet(string key)
        {
            return m_values.Get(key);
        }

        /// <summary>
        /// Returns all values associated with the key as a collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The collection of values for the key.</returns>
        public ICollection<string> GetValues(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            return m_values.GetValues(key);
        }

        /// <summary>
        /// Returns the pairs of keys and objects as a name-value collection.
        /// </summary>
        /// <returns>The name-value collection of keys and objects.</returns>
        public NameValueCollection ToNameValueCollection()
        {
            return new NameValueCollection(m_values);
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
            return m_values.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_values.GetEnumerator();
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
