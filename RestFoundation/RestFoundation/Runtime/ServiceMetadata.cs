using System;

namespace RestFoundation.Runtime
{
    internal struct ServiceMetadata : IEquatable<ServiceMetadata>
    {
        private readonly Type m_type;
        private readonly string m_url;

        public ServiceMetadata(Type type, string url)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            m_type = type;
            m_url = url.Trim();
        }

        public string Url
        {
            get
            {
                return m_url;
            }
        }

        public Type Type
        {
            get
            {
                return m_type;
            }
        }

        public static bool operator ==(ServiceMetadata left, ServiceMetadata right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ServiceMetadata left, ServiceMetadata right)
        {
            return !left.Equals(right);
        }

        public bool Equals(ServiceMetadata other)
        {
            return other.m_type == m_type && Equals(other.m_url, m_url);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ServiceMetadata && Equals((ServiceMetadata) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (m_type.GetHashCode() * 397) ^ m_url.GetHashCode();
            }
        }
    }
}
