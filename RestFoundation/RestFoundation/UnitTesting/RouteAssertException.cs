// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Represents a failed route assert exception.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class RouteAssertException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteAssertException"/> class.
        /// </summary>
        public RouteAssertException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteAssertException"/> class with the provided message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public RouteAssertException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteAssertException"/> class with the provided message
        /// and a reference to the exception that is the cause of the current exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference.</param>
        public RouteAssertException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteAssertException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        /// <exception cref="SerializationException">
        /// The class name is null or <see cref="System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected RouteAssertException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
