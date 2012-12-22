// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Net;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents an HTTP response status for the service proxy.
    /// </summary>
    public struct ProxyStatus : IEquatable<ProxyStatus>
    {
        private readonly HttpStatusCode m_code;
        private readonly string m_condition;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyStatus"/> struct.
        /// </summary>
        /// <param name="code">HTTP status code.</param>
        /// <param name="condition">Condition for the status.</param>
        public ProxyStatus(HttpStatusCode code, string condition)
        {
            if (String.IsNullOrEmpty(condition))
            {
                throw new ArgumentNullException("condition");
            }

            m_code = code;
            m_condition = condition;
        }

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        public HttpStatusCode Code
        {
            get
            {
                return m_code;
            }
        }

        /// <summary>
        /// Gets the condition for the status.
        /// </summary>
        public string Condition
        {
            get
            {
                return m_condition;
            }
        }

        /// <summary>
        /// Compares two <see cref="ProxyStatus"/> objects for equality.
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true if both objects are equivalent; otherwise, false.</returns>
        public static bool operator ==(ProxyStatus left, ProxyStatus right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two <see cref="ProxyStatus"/> objects for inequality.
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true if both objects are not equivalent; otherwise, false.</returns>
        public static bool operator !=(ProxyStatus left, ProxyStatus right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ProxyStatus other)
        {
            return m_code == other.m_code && string.Equals(m_condition, other.m_condition);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ProxyStatus && Equals((ProxyStatus) obj);
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
                return ((int) m_code * 397) ^ m_condition.GetHashCode();
            }
        }
    }
}
