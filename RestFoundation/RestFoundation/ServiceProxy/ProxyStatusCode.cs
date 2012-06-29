using System;
using System.Net;

namespace RestFoundation.ServiceProxy
{
    public struct ProxyStatusCode : IEquatable<ProxyStatusCode>, IComparable<ProxyStatusCode>
    {
        private readonly HttpStatusCode m_statusCode;
        private readonly string m_condition;

        public ProxyStatusCode(HttpStatusCode statusCode, string condition)
        {
            if (String.IsNullOrEmpty(condition)) throw new ArgumentNullException("condition");

            m_statusCode = statusCode;
            m_condition = condition;
        }

        public string Condition
        {
            get
            {
                return m_condition;
            }
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                return m_statusCode;
            }
        }

        public int GetNumericStatusCode()
        {
            return (int) m_statusCode;
        }

        public bool Equals(ProxyStatusCode other)
        {
            return Equals(other.m_statusCode, m_statusCode) && Equals(other.m_condition, m_condition);
        }

        public int CompareTo(ProxyStatusCode other)
        {
            int result = StatusCode.CompareTo(other.StatusCode);

            return result != 0 ? result : String.CompareOrdinal(Condition, other.Condition);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            return obj is ProxyStatusCode && Equals((ProxyStatusCode) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (m_statusCode.GetHashCode() * 397) ^ m_condition.GetHashCode();
            }
        }

        public static bool operator ==(ProxyStatusCode left, ProxyStatusCode right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ProxyStatusCode left, ProxyStatusCode right)
        {
            return !left.Equals(right);
        }
    }
}
