using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace RestFoundation.ServiceProxy.OperationMetadata
{
    /// <summary>
    /// Represents an HTTP response status code metadata.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class StatusCodeMetadata : IComparable<StatusCodeMetadata>, IEquatable<StatusCodeMetadata>
    {
        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        public HttpStatusCode Code { get; set; }

        /// <summary>
        /// Gets or sets the condition for the status.
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// Returns a numeric representation of the HTTP status code.
        /// </summary>
        /// <returns>An <see cref="int"/> representing the status code.</returns>
        public int GetNumericStatusCode()
        {
            return (int) Code;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(StatusCodeMetadata other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Code == other.Code && string.Equals(Condition, other.Condition);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object.</param>
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

            return obj is StatusCodeMetadata && Equals((StatusCodeMetadata) obj);
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
            unchecked
            {
                return ((int) Code * 397) ^ (Condition != null ? Condition.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings:
        /// Value Meaning Less than zero: This object is less than the <paramref name="other"/> parameter.
        /// Zero: This object is equal to <paramref name="other"/>.
        /// Greater than zero: This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(StatusCodeMetadata other)
        {
            return other != null ? Code.CompareTo(other.Code) : 1;
        }
    }
}
