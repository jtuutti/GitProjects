using System;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a service proxy decorator attribute that represents a query string parameter
    /// for a service method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ProxyQueryParameterAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyQueryParameterAttribute"/> class.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="exampleValue">An example parameter value.</param>
        public ProxyQueryParameterAttribute(string name, object exampleValue)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (exampleValue == null) throw new ArgumentNullException("exampleValue");

            Name = name;
            ExampleValue = exampleValue;
        }

        /// <summary>
        /// Gets the parameter comma separated list of all allowed values.
        /// </summary>
        public string AllowedValues { get; set; }

        /// <summary>
        /// Gets the parameter example value.
        /// </summary>
        public object ExampleValue { get; private set; }

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the parameter type.
        /// </summary>
        public Type ParameterType { get; set; }

        /// <summary>
        /// Gets the parameter regular expression constraint.
        /// </summary>
        public string RegexConstraint { get; set; }
    }
}
