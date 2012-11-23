using System;
using System.Net;

namespace RestFoundation.ServiceProxy
{
    public struct ProxyStatus : IEquatable<ProxyStatus>
    {
        private readonly HttpStatusCode m_code;
        private readonly string m_condition;

        public ProxyStatus(HttpStatusCode code, string condition)
        {
            if (String.IsNullOrEmpty(condition))
            {
                throw new ArgumentNullException("condition");
            }

            m_code = code;
            m_condition = condition;
        }

        public HttpStatusCode Code
        {
            get
            {
                return m_code;
            }
        }

        public string Condition
        {
            get
            {
                return m_condition;
            }
        }

        public static bool operator ==(ProxyStatus left, ProxyStatus right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ProxyStatus left, ProxyStatus right)
        {
            return !left.Equals(right);
        }

        public bool Equals(ProxyStatus other)
        {
            return m_code == other.m_code && string.Equals(m_condition, other.m_condition);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ProxyStatus && Equals((ProxyStatus) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) m_code * 397) ^ m_condition.GetHashCode();
            }
        }
    }
}
