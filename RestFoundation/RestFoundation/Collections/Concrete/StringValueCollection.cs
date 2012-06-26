using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;

namespace RestFoundation.Collections.Concrete
{
    [DebuggerDisplay("Count = {Count}")]
    public class StringValueCollection : IStringValueCollection
    {
        private readonly NameValueCollection m_values;

        public ICollection<string> Keys
        {
            get
            {
                return new ReadOnlyCollection<string>(m_values.AllKeys);
            }
        }

        public int Count
        {
            get
            {
                return m_values.Count;
            }
        }

        public StringValueCollection(NameValueCollection values)
        {
            if (values == null) throw new ArgumentNullException("values");

            m_values = values;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return new StringValueEnumerator(m_values.GetEnumerator());
        }

        public string Get(string key)
        {
            string value = TryGet(key);
            if (value == null) throw new ArgumentOutOfRangeException("key", "Invalid key provided");

            return value;
        }

        public string TryGet(string key)
        {
            return m_values.Get(key);
        }

        public ICollection<string> GetValues(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            return m_values.GetValues(key);
        }

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
            private readonly IEnumerator enumerator;

            public StringValueEnumerator(IEnumerator enumerator)
            {
                this.enumerator = enumerator;
            }

            public string Current
            {
                get
                {
                    return (string) enumerator.Current;
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
                return enumerator.MoveNext();
            }

            public void Reset()
            {
                enumerator.Reset();
            }

            public void Dispose()
            {
            }
        }
    }
}
