using System;

namespace RestFoundation.ServiceProxy
{
    public struct ProxyHeader : IEquatable<ProxyHeader>
    {
        private readonly string m_name;
        private readonly string m_value;

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

        public string Name
        {
            get
            {
                return m_name;
            }
        }

        public string Value
        {
            get
            {
                return m_value;
            }
        }

        public static bool operator ==(ProxyHeader left, ProxyHeader right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ProxyHeader left, ProxyHeader right)
        {
            return !left.Equals(right);
        }

        public bool Equals(ProxyHeader other)
        {
            return string.Equals(m_name, other.m_name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is ProxyHeader && Equals((ProxyHeader) obj);
        }

        public override int GetHashCode()
        {
            return m_name.GetHashCode();
        }
    }
}
