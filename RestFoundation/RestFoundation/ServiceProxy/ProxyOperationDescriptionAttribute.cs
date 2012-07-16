using System;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a service proxy decorator attribute that specifies a description for a
    /// service method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ProxyOperationDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyOperationDescriptionAttribute"/> class.
        /// </summary>
        /// <param name="description">The service method description.</param>
        public ProxyOperationDescriptionAttribute(string description)
        {
            if (description == null)
            {
                throw new ArgumentNullException("description");
            }

            Description = description;
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; private set; }
    }
}
