using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;

namespace RestFoundation
{
    /// <summary>
    /// Represents a service runtime aggregate exception. It should not be thrown from
    /// the user code.
    /// </summary>
    [Serializable]
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors",
                     Justification = "This exception is not stand-alone but aggregates other exceptions")]
    public sealed class ServiceRuntimeException : Exception
    {
        private const string DefaultMessage = "A service exception occurred";

        private readonly ReadOnlyCollection<Exception> m_innerExceptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRuntimeException"/> class.
        /// </summary>
        public ServiceRuntimeException()
        {
            m_innerExceptions = new ReadOnlyCollection<Exception>(new Exception[0]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRuntimeException"/> class with the provided
        /// inner exceptions.
        /// </summary>
        /// <param name="innerExceptions">An array of inner exceptions.</param>
        public ServiceRuntimeException(params Exception[] innerExceptions)
            : base(innerExceptions != null && innerExceptions.Length > 0 ? innerExceptions[0].Message : DefaultMessage,
                   innerExceptions != null && innerExceptions.Length > 0 ? innerExceptions[0] : null)
        {
            m_innerExceptions = innerExceptions != null ? new ReadOnlyCollection<Exception>(innerExceptions) : new ReadOnlyCollection<Exception>(new Exception[0]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRuntimeException"/> class with the provided
        /// inner exceptions.
        /// </summary>
        /// <param name="innerExceptions">A sequence of inner exceptions.</param>
        public ServiceRuntimeException(IEnumerable<Exception> innerExceptions)
            : this(innerExceptions != null ? innerExceptions.ToArray() : new Exception[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRuntimeException"/> class with the provided
        /// message and inner exceptions.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerExceptions">An array of inner exceptions.</param>
        public ServiceRuntimeException(string message, params Exception[] innerExceptions)
            : base(message, innerExceptions != null && innerExceptions.Length > 0 ? innerExceptions[0] : null)
        {
            m_innerExceptions = innerExceptions != null ? new ReadOnlyCollection<Exception>(innerExceptions) : new ReadOnlyCollection<Exception>(new Exception[0]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRuntimeException"/> class with the provided
        /// message and inner exceptions.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerExceptions">A sequence of inner exceptions.</param>
        public ServiceRuntimeException(string message, IEnumerable<Exception> innerExceptions)
            : this(message, innerExceptions != null ? innerExceptions.ToArray() : new Exception[0])
        {
        }

        [SecurityCritical]
        private ServiceRuntimeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            var innerExceptionArray = info.GetValue("InnerExceptions", typeof(Exception[])) as Exception[];

            if (innerExceptionArray != null)
            {
                m_innerExceptions = new ReadOnlyCollection<Exception>(innerExceptionArray);
            }
            else
            {
                m_innerExceptions = new ReadOnlyCollection<Exception>(new Exception[0]);
            }
        }

        /// <summary>
        /// Gets a sequence of inner exception.
        /// </summary>
        public IEnumerable<Exception> InnerExceptions
        {
            get
            {
                return m_innerExceptions;
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
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            base.GetObjectData(info, context);

            var innerExceptionArray = new Exception[m_innerExceptions.Count];
            m_innerExceptions.CopyTo(innerExceptionArray, 0);

            info.AddValue("InnerExceptions", innerExceptionArray, typeof(Exception[]));
        }
    }
}
