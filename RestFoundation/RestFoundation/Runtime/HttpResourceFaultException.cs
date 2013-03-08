// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents an HTTP resource fault exception. This is a special type of exception that is used
    /// to return a collection of resource validation faults to the REST client. It does not get caught
    /// by the service behaviors.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public sealed class HttpResourceFaultException : Exception
    {
        private readonly List<string> m_faultMessages = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResourceFaultException"/> class.
        /// </summary>
        public HttpResourceFaultException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResourceFaultException"/> class with the provided general fault
        /// messages.
        /// </summary>
        /// <param name="faultMessages">A sequence of fault messages.</param>
        public HttpResourceFaultException(IEnumerable<string> faultMessages)
        {
            if (faultMessages == null)
            {
                throw new ArgumentNullException("faultMessages");
            }

            m_faultMessages.AddRange(faultMessages.Select(x => x.Replace("\r", String.Empty).Replace("\n", " ")));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResourceFaultException"/> class with the provided message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public HttpResourceFaultException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResourceFaultException"/> class with the provided message
        /// and a reference to the exception that is the cause of the current exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference.</param>
        public HttpResourceFaultException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResourceFaultException"/> class with serialized data.
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
        private HttpResourceFaultException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            m_faultMessages = info.GetValue("faultMessages", typeof(List<string>)) as List<string>;
        }

        /// <summary>
        /// Gets a list of the exception specific general fault messages.
        /// </summary>
        public IEnumerable<string> FaultMessages
        {
            get
            {
                return m_faultMessages;
            }
        }

        /// <summary>
        /// Sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("faultMessages", FaultMessages);
        }
    }
}
