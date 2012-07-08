using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Web;

namespace RestFoundation.Collections.Concrete
{
    [DebuggerDisplay("Count = {Count}")]
    public class CookieValueCollection : ICookieValueCollection
    {
        private readonly HttpCookieCollection m_values;

        internal CookieValueCollection(HttpCookieCollection values)
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

        public IEnumerator<HttpCookie> GetEnumerator()
        {
            return new CookieValueEnumerator(m_values.GetEnumerator());
        }

        public HttpCookie Get(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            HttpCookie value = TryGet(key);
            if (value == null) throw new ArgumentOutOfRangeException("key", "Invalid key provided");

            return value;
        }

        public HttpCookie TryGet(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

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
