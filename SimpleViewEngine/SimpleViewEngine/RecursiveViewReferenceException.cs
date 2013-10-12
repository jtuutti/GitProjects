using System;
using System.Runtime.Serialization;

namespace SimpleViewEngine
{
    /// <summary>
    /// Represents an exception thrown when an <see cref="HtmlView"/> file references itself
    /// recursively.
    /// </summary>
    [Serializable]
    public class RecursiveViewReferenceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecursiveViewReferenceException"/> class.
        /// </summary>
        public RecursiveViewReferenceException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecursiveViewReferenceException"/> class
        /// with the provided message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public RecursiveViewReferenceException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecursiveViewReferenceException"/> class
        /// with the provided message and a reference to the exception that is the cause of the
        /// current exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference.
        /// </param>
        public RecursiveViewReferenceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecursiveViewReferenceException"/> class
        /// with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the
        /// exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source
        /// or destination.
        /// </param>
        /// <exception cref="SerializationException">
        /// The class name is null or <see cref="System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected RecursiveViewReferenceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
