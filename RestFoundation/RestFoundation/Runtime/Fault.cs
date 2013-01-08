// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.ComponentModel;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents a general or a resource fault.
    /// </summary>
    public class Fault
    {
        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the fault message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// A serializer helper method.
        /// </summary>
        /// <returns>A value indicating whether the property name should be serialized.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializePropertyName()
        {
            return !String.IsNullOrEmpty(PropertyName);
        }
    }
}
