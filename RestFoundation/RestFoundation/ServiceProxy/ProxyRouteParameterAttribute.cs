using System;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a service proxy decorator attribute for a service method parameter that specifies
    /// a route parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class ProxyRouteParameterAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyRouteParameterAttribute"/> class.
        /// </summary>
        /// <param name="exampleValue">An example parameter value.</param>
        public ProxyRouteParameterAttribute(object exampleValue)
        {
            if (exampleValue == null) throw new ArgumentNullException("exampleValue");

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
    }
}
