using System;

namespace RestFoundation.ServiceProxy
{
    public struct ProxyParameter : IEquatable<ProxyParameter>
    {
        private readonly string m_name;
        private readonly string m_type;
        private readonly string m_constraint;
        private readonly object m_exampleValue;
        private readonly string m_allowedValues;
        private readonly bool m_isRouteParameter;

        public ProxyParameter(string name, string type, string constraint, bool isRouteParameter) : this(name, type, constraint, null, null, isRouteParameter)
        {
        }

        public ProxyParameter(string name, string type, string constraint, object exampleValue, string allowedValues, bool isRouteParameter)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (String.IsNullOrEmpty(type)) throw new ArgumentNullException("type");

            m_name = name;
            m_type = type;
            m_constraint = constraint;
            m_exampleValue = exampleValue;
            m_allowedValues = allowedValues;
            m_isRouteParameter = isRouteParameter;
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

        public string Constraint
        {
            get
            {
                return m_constraint;
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

        public bool IsRouteParameter
        {
            get
            {
                return m_isRouteParameter;
            }
        }

        public bool Equals(ProxyParameter other)
        {
            return Equals(other.m_name, m_name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            return obj is ProxyParameter && Equals((ProxyParameter) obj);
        }

        public override int GetHashCode()
        {
            return m_name.GetHashCode();
        }

        public static bool operator ==(ProxyParameter left, ProxyParameter right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ProxyParameter left, ProxyParameter right)
        {
            return !left.Equals(right);
        }
    }
}
