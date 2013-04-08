// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestFoundation.Collections.Specialized
{
    internal sealed class HttpMethodCollection
    {
        private readonly HashSet<HttpMethod> m_methods;

        public HttpMethodCollection()
        {
            m_methods = new HashSet<HttpMethod>();
        }

        public bool IsFinalized { get; private set; }

        public bool Add(HttpMethod method)
        {
            return !IsFinalized && m_methods.Add(method);
        }

        public void AddRange(IEnumerable<HttpMethod> methods)
        {
            if (methods == null)
            {
                throw new ArgumentNullException("methods");
            }

            if (IsFinalized)
            {
                return;
            }

            foreach (HttpMethod method in methods)
            {
                m_methods.Add(method);
            }
        }

        public void MarkFinalized()
        {
            IsFinalized = true;
        }

        public override string ToString()
        {
            return String.Join(", ", m_methods.Select(m => m.ToString()).OrderBy(m => m)).ToUpperInvariant();
        }
    }
}
