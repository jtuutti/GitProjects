// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a service proxy parameter.
    /// </summary>
    public struct ProxyParameter : IEquatable<ProxyParameter>
    {
        private readonly string m_name;
        private readonly string m_parameterType;
        private readonly string m_constraint;
        private readonly object m_exampleValue;
        private readonly string m_allowedValues;
        private readonly bool m_isRouteParameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyParameter"/> struct.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="parameterType">The parameter type.</param>
        /// <param name="constraint">The paramater regular expression contraint.</param>
        /// <param name="isRouteParameter">The value indicating whether the parameter is a route parameter.</param>
        public ProxyParameter(string name, string parameterType, string constraint, bool isRouteParameter) : this(name, parameterType, constraint, null, null, isRouteParameter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyParameter"/> struct.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="parameterType">The parameter type.</param>
        /// <param name="constraint">The paramater regular expression contraint.</param>
        /// <param name="exampleValue">An example parameter value.</param>
        /// <param name="allowedValues">A comma separated list of all allowed parameter values, if applicable.</param>
        /// <param name="isRouteParameter">The value indicating whether the parameter is a route parameter.</param>
        public ProxyParameter(string name, string parameterType, string constraint, object exampleValue, string allowedValues, bool isRouteParameter)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (String.IsNullOrEmpty(parameterType))
            {
                throw new ArgumentNullException("parameterType");
            }

            m_name = name;
            m_parameterType = parameterType;
            m_constraint = constraint;
            m_exampleValue = exampleValue;
            m_allowedValues = allowedValues;
            m_isRouteParameter = isRouteParameter;
        }

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        public string Name
        {
            get
            {
                return m_name;
            }
        }

        /// <summary>
        /// Gets the parameter type.
        /// </summary>
        public string ParameterType
        {
            get
            {
                return m_parameterType;
            }
        }

        /// <summary>
        /// Gets the parameter regular expression constraint.
        /// </summary>
        public string Constraint
        {
            get
            {
                return m_constraint;
            }
        }

        /// <summary>
        /// Gets the parameter example value.
        /// </summary>
        public object ExampleValue
        {
            get
            {
                return m_exampleValue;
            }
        }

        /// <summary>
        /// Gets the parameter comma separated list of all allowed values.
        /// </summary>
        public string AllowedValues
        {
            get
            {
                return m_allowedValues;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the parameter is a route parameter.
        /// </summary>
        public bool IsRouteParameter
        {
            get
            {
                return m_isRouteParameter;
            }
        }

        /// <summary>
        /// Compares two <see cref="ProxyParameter"/> objects for equality.
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true if both objects are equivalent; otherwise, false.</returns>
        public static bool operator ==(ProxyParameter left, ProxyParameter right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two <see cref="ProxyParameter"/> objects for inequality.
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true if both objects are not equivalent; otherwise, false.</returns>
        public static bool operator !=(ProxyParameter left, ProxyParameter right)
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
        public bool Equals(ProxyParameter other)
        {
            return Equals(other.m_name, m_name);
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

            return obj is ProxyParameter && Equals((ProxyParameter) obj);
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
