using System;
using System.Runtime.Serialization;

namespace RestFoundation.DependencyInjection
{
    /// <summary>
    /// Represents a dependency injection exception.
    /// </summary>
    [Serializable]
    public class DependencyInjectionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionException"/> class.
        /// </summary>
        public DependencyInjectionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionException"/> class with the provided message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public DependencyInjectionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionException"/> class with the provided message
        /// and a reference to the exception that is the cause of the current exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference.</param>
        public DependencyInjectionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionException"/> class with serialized data.
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
        protected DependencyInjectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
