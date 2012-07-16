using System;
using System.Net;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents an HTTP status code to display in the service proxy UI.
    /// </summary>
    public struct ProxyStatusCode : IEquatable<ProxyStatusCode>, IComparable<ProxyStatusCode>
    {
        private readonly HttpStatusCode m_statusCode;
        private readonly string m_condition;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyStatusCode"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="condition">The condition when the status code may occur.</param>
        public ProxyStatusCode(HttpStatusCode statusCode, string condition)
        {
            if (String.IsNullOrEmpty(condition)) throw new ArgumentNullException("condition");

            m_statusCode = statusCode;
            m_condition = condition;
        }

        /// <summary>
        /// Gets the condition under which the HTTP status code may occur.
        /// </summary>
        public string Condition
        {
            get
            {
                return m_condition;
            }
        }

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get
            {
                return m_statusCode;
            }
        }

        /// <summary>
        /// Gets a numeric HTTP status code.
        /// </summary>
        public int GetNumericStatusCode()
        {
            return (int) m_statusCode;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following
        /// meanings: Less than zero - This object is less than the <paramref name="other"/> parameter. Zero -
        /// This object is equal to <paramref name="other"/>. Greater than zero - This object is greater than
        /// <paramref name="other"/>.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(ProxyStatusCode other)
        {
            int result = StatusCode.CompareTo(other.StatusCode);

            return result != 0 ? result : String.CompareOrdinal(Condition, other.Condition);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ProxyStatusCode other)
        {
            return Equals(other.m_statusCode, m_statusCode) && Equals(other.m_condition, m_condition);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to.</param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            return obj is ProxyStatusCode && Equals((ProxyStatusCode) obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return (m_statusCode.GetHashCode() * 397) ^ m_condition.GetHashCode();
            }
        }

        /// <summary>
        /// Compares two <see cref="ProxyStatusCode"/> objects for equality.
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true if both objects are equivalent; otherwise, false.</returns>
        public static bool operator ==(ProxyStatusCode left, ProxyStatusCode right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares whether the first <see cref="ProxyStatusCode"/> is less than the second.
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true if the first object is less than the second; otherwise, false.</returns>
        public static bool operator !=(ProxyStatusCode left, ProxyStatusCode right)
        {
            return !left.Equals(right);
        }
        /// <summary>
        /// Compares whether the first <see cref="ProxyStatusCode"/> is less than the second.
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true if the first object is less than the second; otherwise, false.</returns>
        public static bool operator <(ProxyStatusCode left, ProxyStatusCode right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Compares whether the first <see cref="ProxyStatusCode"/> is greater than the second.
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true if the first object is greater than the second; otherwise, false.</returns>
        public static bool operator >(ProxyStatusCode left, ProxyStatusCode right)
        {
            return left.CompareTo(right) > 0;
        }
    }
}
