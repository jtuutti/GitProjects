using System;

namespace RestFoundation.Behaviors.Attributes
{
    /// <summary>
    /// Represents service method execution and result processing timeouts.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ServiceMethodTimeoutAttribute : Attribute
    {
        private int? m_resultTimeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMethodTimeoutAttribute"/> class.
        /// </summary>
        /// <param name="serviceTimeoutInSeconds">
        /// The service method execution timeout value in seconds. Set the value to <see cref="TimeSpan.Zero"/>
        /// for an infinite timeout.
        /// </param>
        public ServiceMethodTimeoutAttribute(int serviceTimeoutInSeconds)
        {
            if (serviceTimeoutInSeconds < -1)
            {
                throw new ArgumentOutOfRangeException("serviceTimeoutInSeconds", RestResources.InvalidServiceMethodTimeout);
            }

            ServiceTimeoutInSeconds = serviceTimeoutInSeconds > 0 ? serviceTimeoutInSeconds : 0;
        }

        /// <summary>
        /// Gets the service method execution timeout value in seconds.
        /// </summary>
        public int ServiceTimeoutInSeconds { get; private set; }

        /// <summary>
        /// Gets or sets the service method execution timeout value in seconds. Set the value to <see cref="TimeSpan.Zero"/>
        /// for an infinite timeout. A null value means the default timeout value of 30 seconds.
        /// </summary>
        public int? ResultTimeoutInSeconds
        {
            get
            {
                return m_resultTimeout;
            }
            set
            {
                if (!value.HasValue)
                {
                    m_resultTimeout = null;
                    return;
                }

                if (value.Value < -1)
                {
                    throw new ArgumentOutOfRangeException("value", RestResources.InvalidServiceMethodTimeout);
                }

                m_resultTimeout = value.Value > 0 ? value.Value : 0;
            }
        }
    }
}
