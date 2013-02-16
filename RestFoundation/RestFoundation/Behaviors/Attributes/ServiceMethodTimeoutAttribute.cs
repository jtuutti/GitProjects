using System;

namespace RestFoundation.Behaviors.Attributes
{
    /// <summary>
    /// Represents service method execution and result processing timeouts.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ServiceMethodTimeoutAttribute : Attribute
    {
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
    }
}
