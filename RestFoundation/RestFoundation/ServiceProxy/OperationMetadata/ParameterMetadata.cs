using System;

namespace RestFoundation.ServiceProxy.OperationMetadata
{
    /// <summary>
    /// Represents a service operation parameter metadata.
    /// </summary>
    public sealed class ParameterMetadata : IEquatable<ParameterMetadata>
    {
        /// <summary>
        /// Gets or sets the parameter name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the parameter type.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parameter is a route parameter (true),
        /// or a query string parameter (false).
        /// </summary>
        public bool IsRouteParameter { get; set; }

        /// <summary>
        /// Gets or sets a parameter example value.
        /// </summary>
        public object ExampleValue { get; set; }

        /// <summary>
        /// Gets or sets a comma separated sequence of allowed parameter values.
        /// </summary>
        public string AllowedValues { get; set; }

        /// <summary>
        /// Gets or sets a regular expression constraint.
        /// </summary>
        public string RegexConstraint { get; set; }

        /// <summary>
        /// Gets the parameter type description for the service proxy.
        /// </summary>
        /// <returns>The parameter type description.</returns>
        public string GetTypeDescription()
        {
            return TypeDescriptor.GetTypeName(Type);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ParameterMetadata other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return ReferenceEquals(this, other) || string.Equals(Name, other.Name);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is ParameterMetadata && Equals((ParameterMetadata) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }
    }
}