﻿using System;

namespace RestFoundation
{
    /// <summary>
    /// Represents a validation error.
    /// </summary>
    public struct ValidationError : IEquatable<ValidationError>
    {
        private readonly string m_propertyName;
        private readonly string m_message;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationError"/> struct.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="message">The error message.</param>
        public ValidationError(string propertyName, string message) : this()
        {
            m_propertyName = propertyName;
            m_message = message;
        }

        /// <summary>
        /// Gets the property name for the error.
        /// </summary>
        public string PropertyName
        {
            get
            {
                return m_propertyName;
            }
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string Message
        {
            get
            {
                return m_message;
            }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ValidationError other)
        {
            return Equals(other.m_propertyName, m_propertyName) && Equals(other.m_message, m_message);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            return obj is ValidationError && Equals((ValidationError) obj);
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
            unchecked
            {
                return ((m_propertyName != null ? m_propertyName.GetHashCode() : 0) * 397) ^ (m_message != null ? m_message.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Represents an operator semantically equivalent to the <see cref="Equals(object)"/> method.
        /// </summary>
        /// <param name="left">The first validation error object to compare.</param>
        /// <param name="right">The second validation error object to compare.</param>
        /// <returns></returns>
        public static bool operator ==(ValidationError left, ValidationError right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Represents an operator semantically equivalent to the <see cref="Equals(object)"/> method,
        /// but the returned value is negated.
        /// </summary>
        /// <param name="left">The first validation error object to compare.</param>
        /// <param name="right">The second validation error object to compare.</param>
        /// <returns></returns>
        public static bool operator !=(ValidationError left, ValidationError right)
        {
            return !left.Equals(right);
        }
    }
}
