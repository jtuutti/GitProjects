// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents a Link HTTP header value.
    /// </summary>
    public struct Link : IEquatable<Link>
    {
        private readonly Uri m_href;
        private readonly string m_rel;
        private readonly string m_anchor;
        private readonly string m_title;
        private readonly IDictionary<string, string> m_additionalParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="Link"/> struct.
        /// </summary>
        /// <param name="href">An absolute or relative URL.</param>
        /// <param name="rel">A relation value.</param>
        /// <param name="anchor">An optional URL anchor.</param>
        /// <param name="title">An optional title value.</param>
        /// <param name="additionalParameters">Additional link parameters</param>
        public Link(Uri href, string rel, string anchor, string title, IDictionary<string, string> additionalParameters)
        {
            if (href == null)
            {
                throw new ArgumentNullException("rel");
            }

            if (String.IsNullOrWhiteSpace(rel))
            {
                throw new ArgumentNullException("rel");
            }

            m_href = href;
            m_rel = rel;
            m_anchor = anchor;
            m_title = title;
            m_additionalParameters = additionalParameters ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the link URL.
        /// </summary>
        public Uri Href
        {
            get
            {
                return m_href;
            }
        }

        /// <summary>
        /// Gets the link relation to the resource.
        /// </summary>
        public string Rel
        {
            get
            {
                return m_rel;
            }
        }

        /// <summary>
        /// Gets the URL anchor.
        /// </summary>
        public string Anchor
        {
            get
            {
                return m_anchor;
            }
        }

        /// <summary>
        /// Gets the link title.
        /// </summary>
        public string Title
        {
            get
            {
                return m_title;
            }
        }

        /// <summary>
        /// Compares two <see cref="Link"/> objects for equality.
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true if both objects are equivalent; otherwise, false.</returns>
        public static bool operator ==(Link left, Link right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two <see cref="Link"/> objects for inequality.
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true if both objects are not equivalent; otherwise, false.</returns>
        public static bool operator !=(Link left, Link right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Gets additional an additional parameter specified in the Link header.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <returns>The parameter value.</returns>
        public string GetAdditionalParameter(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            string value;
            return m_additionalParameters.TryGetValue(name, out value) ? value : null;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Link other)
        {
            return string.Equals(m_href, other.m_href) && string.Equals(m_rel, other.m_rel);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is Link && Equals((Link) obj);
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
                return ((m_href != null ? m_href.GetHashCode() : 0) * 397) ^ (m_rel != null ? m_rel.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "href: {0}, rel: {1}", m_href, m_rel);
        }
    }
}
