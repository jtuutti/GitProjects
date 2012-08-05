// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Runtime.Serialization;

namespace RestFoundation.ServiceLocation
{
    /// <summary>
    /// Represents an exception that gets thrown if service location results in an error.
    /// </summary>
    [Serializable]
    public class ServiceActivationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceActivationException"/> class.
        /// </summary>
        public ServiceActivationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceActivationException"/> class with the provided message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ServiceActivationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceActivationException"/> class with the provided message
        /// and a reference to the exception that is the cause of the current exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference.</param>
        public ServiceActivationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceActivationException"/> class with serialized data.
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
        protected ServiceActivationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
