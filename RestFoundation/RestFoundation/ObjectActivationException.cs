using System;
using System.Runtime.Serialization;

namespace RestFoundation
{
    /// <summary>
    /// Represents an object activation exception.
    /// </summary>
    [Serializable]
    public class ObjectActivationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectActivationException"/> class.
        /// </summary>
        public ObjectActivationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectActivationException"/> class with the provided message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ObjectActivationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectActivationException"/> class with the provided message
        /// and a reference to the exception that is the cause of the current exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference.</param>
        public ObjectActivationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectActivationException"/> class with serialized data.
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
        protected ObjectActivationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
