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

        internal StringValueCollection(NameValueCollection values)
        {
            if (values == null) throw new ArgumentNullException("values");

            m_values = values;
        }

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

        public NameValueCollection ToNameValueCollection()
        {
            return new NameValueCollection(m_values);
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
