// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;

namespace RestFoundation.Runtime
{
    internal struct RouteMetadata : IEquatable<RouteMetadata>
    {
        private readonly string m_typeName;
        private readonly string m_urlTemplate;
        private readonly string m_standardizedUrlTemplate;

        public RouteMetadata(string typeName, string urlTemplate)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }

            if (urlTemplate == null)
            {
                throw new ArgumentNullException("urlTemplate");
            }

            m_typeName = typeName;
            m_urlTemplate = urlTemplate;
            m_standardizedUrlTemplate = UrlTemplateStandardizer.Standardize(urlTemplate);
        }

        public string TypeName
        {
            get
            {
                return m_typeName;
            }
        }

        public string UrlTemplate
        {
            get
            {
                return m_urlTemplate;
            }
        }

        public string StandardizedUrlTemplate
        {
            get
            {
                return m_standardizedUrlTemplate;
            }
        }

        public static bool operator ==(RouteMetadata left, RouteMetadata right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RouteMetadata left, RouteMetadata right)
        {
            return !left.Equals(right);
        }

        public bool Equals(RouteMetadata other)
        {
            return Equals(other.m_typeName, m_typeName) && Equals(other.m_standardizedUrlTemplate, m_standardizedUrlTemplate);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is RouteMetadata && Equals((RouteMetadata) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (m_typeName.GetHashCode() * 397) ^ m_standardizedUrlTemplate.GetHashCode();
            }
        }
    }
}
