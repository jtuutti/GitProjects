using System;

namespace RestFoundation.ServiceProxy.Helpers
{
    public struct ProxyRouteParameter : IEquatable<ProxyRouteParameter>
    {
        private readonly string m_name;
        private readonly string m_type;
        private readonly object m_exampleValue;
        private readonly string m_allowedValues;

        public ProxyRouteParameter(string name, string type) : this(name, type, null, null)
        {
        }

        public ProxyRouteParameter(string name, string type, object exampleValue, string allowedValues)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (String.IsNullOrEmpty(type)) throw new ArgumentNullException("type");

            m_name = name;
            m_type = type;
            m_exampleValue = exampleValue;
            m_allowedValues = allowedValues;
        }

        public string Name
        {
            get
            {
                return m_name;
            }
        }

        public string Type
        {
            get
            {
                return m_type;
            }
        }

        public object ExampleValue
        {
            get
            {
                return m_exampleValue;
            }
        }

        public string AllowedValues
        {
            get
            {
                return m_allowedValues;
            }
        }

        public bool Equals(ProxyRouteParameter other)
        {
            return Equals(other.m_name, m_name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            return obj is ProxyRouteParameter && Equals((ProxyRouteParameter) obj);
        }

        public override int GetHashCode()
        {
            return m_name.GetHashCode();
        }

        public static bool operator ==(ProxyRouteParameter left, ProxyRouteParameter right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ProxyRouteParameter left, ProxyRouteParameter right)
        {
            return !left.Equals(right);
        }
    }
}
