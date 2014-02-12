// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace RestFoundation
{
    /// <summary>
    /// Represents a Link HTTP header value.
    /// </summary>
    public class Link : IEquatable<Link>
    {
        private readonly IDictionary<string, string> m_additionalParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="Link"/> class.
        /// </summary>
        /// <param name="href">An absolute or relative URL.</param>
        /// <param name="rel">A relation value.</param>
        public Link(Uri href, string rel) : this(href, rel, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Link"/> class.
        /// </summary>
        /// <param name="href">An absolute or relative URL.</param>
        /// <param name="rel">A relation value.</param>
        /// <param name="anchor">An optional URL anchor.</param>
        /// <param name="title">An optional title value.</param>
        public Link(Uri href, string rel, string anchor, string title) : this(href, rel, anchor, title, null)
        {
        }

        internal Link(Uri href, string rel, string anchor, string title, IDictionary<string, string> additionalParameters)
        {
            if (href == null)
            {
                throw new ArgumentNullException("href");
            }

            Href = href.ToString();
            Rel = !String.IsNullOrWhiteSpace(rel) ? rel : "self";
            Anchor = anchor;
            Title = title;
            m_additionalParameters = additionalParameters ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Link"/> class.
        /// </summary>
        protected Link()
        {
        }

        /// <summary>
        /// Gets or sets the link URL.
        /// </summary>
        [XmlAttribute("href")]
        [JsonProperty(PropertyName = "href", NullValueHandling = NullValueHandling.Ignore)]
        public string Href { get; set; }

        /// <summary>
        /// Gets the link relation to the resource.
        /// </summary>
        [XmlAttribute("rel")]
        [JsonProperty(PropertyName = "rel", NullValueHandling = NullValueHandling.Ignore)]
        public string Rel { get; set; }

        /// <summary>
        /// Gets the URL anchor.
        /// </summary>
        [XmlAttribute("anchor")]
        [JsonProperty(PropertyName = "anchor", NullValueHandling = NullValueHandling.Ignore)]
        public string Anchor { get; set; }

        /// <summary>
        /// Gets the link title.
        /// </summary>
        [XmlAttribute("title")]
        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

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
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return ReferenceEquals(this, other) || Equals(Href, other.Href);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((Link) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Href != null ? Href.GetHashCode() : 0;
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
            return String.Format(CultureInfo.InvariantCulture, "href: {0}, rel: {1}", Href, Rel);
        }
    }
}
