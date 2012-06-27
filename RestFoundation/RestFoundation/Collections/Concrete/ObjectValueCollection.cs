using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace RestFoundation.Collections.Concrete
{
    [DebuggerDisplay("Count = {Count}")]
    public class ObjectValueCollection : IObjectValueCollection
    {
        private readonly IDictionary<string, object> m_values;

        public ObjectValueCollection() : this(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase))
        {
        }

        internal ObjectValueCollection(IDictionary<string, object> values)
        {
            if (values == null) throw new ArgumentNullException("values");

            m_values = values;
        }

        public ICollection<string> Keys
        {
            get
            {
                return new ReadOnlyCollection<string>(new List<string>(m_values.Keys));
            }
        }

        public int Count
        {
            get
            {
                return m_values.Count;
            }
        }

        public IEnumerator<object> GetEnumerator()
        {
            return new ObjectValueEnumerator(m_values.GetEnumerator());
        }

        public object Get(string key)
        {
            object value = TryGet(key);
            if (value == null) throw new ArgumentOutOfRangeException("key", "Invalid key provided");

            return value;
        }

        public object TryGet(string key)
        {
            object value;
            return m_values.TryGetValue(key, out value) ? value : null;
        }

        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>(m_values, StringComparer.OrdinalIgnoreCase);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_values.GetEnumerator();
        }

        private class ObjectValueEnumerator : IEnumerator<object>
        {
            private readonly IEnumerator enumerator;

            public ObjectValueEnumerator(IEnumerator enumerator)
            {
                this.enumerator = enumerator;
            }

            public object Current
            {
                get
                {
                    return enumerator.Current;
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
