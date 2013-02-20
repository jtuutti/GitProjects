using System;

namespace RestFoundation.Behaviors.Attributes
{
    /// <summary>
    /// Represents service method execution and result processing timeouts.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class AsyncTimeoutAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTimeoutAttribute"/> class.
        /// </summary>
        /// <param name="timeoutInSeconds">
        /// A timeout for an asynchronous task returned by the service method in seconds.
        /// Set the value to <see cref="TimeSpan.Zero"/> for an infinite timeout.
        /// </param>
        public AsyncTimeoutAttribute(int timeoutInSeconds)
        {
            if (timeoutInSeconds < -1)
            {
                throw new ArgumentOutOfRangeException("timeoutInSeconds", RestResources.InvalidAsyncTimeout);
            }

            TimeoutInSeconds = timeoutInSeconds > 0 ? timeoutInSeconds : 0;
        }

        /// <summary>
        /// Gets the service method execution timeout value in seconds.
        /// </summary>
        public int TimeoutInSeconds { get; private set; }
    }
}
