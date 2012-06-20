using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;

namespace RestFoundation
{
    [Serializable]
    public class ServiceRuntimeException : Exception
    {
        private const string DefaultMessage = "A service exception occurred";

        private readonly ReadOnlyCollection<Exception> m_innerExceptions;

        public IEnumerable<Exception> InnerExceptions
        {
            get
            {
                return m_innerExceptions;
            }
        }

        public ServiceRuntimeException(params Exception[] innerExceptions)
            : base(innerExceptions != null && innerExceptions.Length > 0 ? innerExceptions[0].Message : DefaultMessage,
                   innerExceptions != null && innerExceptions.Length > 0 ? innerExceptions[0] : null)
        {
            m_innerExceptions = innerExceptions != null ? new ReadOnlyCollection<Exception>(innerExceptions) : new ReadOnlyCollection<Exception>(new Exception[0]);
        }

        public ServiceRuntimeException(IEnumerable<Exception> innerExceptions)
            : this(innerExceptions != null ? innerExceptions.ToArray() : new Exception[0])
        {
        }

        public ServiceRuntimeException(string message, params Exception[] innerExceptions)
            : base(message, innerExceptions != null && innerExceptions.Length > 0 ? innerExceptions[0] : null)
        {
            m_innerExceptions = innerExceptions != null ? new ReadOnlyCollection<Exception>(innerExceptions) : new ReadOnlyCollection<Exception>(new Exception[0]);
        }

        public ServiceRuntimeException(string message, IEnumerable<Exception> innerExceptions)
            : this(message, innerExceptions != null ? innerExceptions.ToArray() : new Exception[0])
        {
        }

        [SecurityCritical]
        protected ServiceRuntimeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
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
