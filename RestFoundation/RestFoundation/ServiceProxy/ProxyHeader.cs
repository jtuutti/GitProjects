// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents an HTTP header.
    /// </summary>
    public struct ProxyHeader : IEquatable<ProxyHeader>
    {
        private readonly string m_name;
        private readonly string m_value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyHeader"/> struct.
        /// </summary>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value.</param>
        public ProxyHeader(string name, string value)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            m_name = name;
            m_value = value;
        }

        /// <summary>
        /// Gets the header name.
        /// </summary>
        public string Name
        {
            get
            {
                return m_name;
            }
        }

        /// <summary>
        /// Gets the header value.
        /// </summary>
        public string Value
        {
            get
            {
                return m_value;
            }
        }

        /// <summary>
        /// Compares two <see cref="ProxyHeader"/> objects for equality.
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true if both objects are equivalent; otherwise, false.</returns>
        public static bool operator ==(ProxyHeader left, ProxyHeader right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two <see cref="ProxyHeader"/> objects for inequality.
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true if both objects are not equivalent; otherwise, false.</returns>
        public static bool operator !=(ProxyHeader left, ProxyHeader right)
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
        public bool Equals(ProxyHeader other)
        {
            return string.Equals(m_name, other.m_name);
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
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is ProxyHeader && Equals((ProxyHeader) obj);
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
            return m_name.GetHashCode();
        }
    }
}
