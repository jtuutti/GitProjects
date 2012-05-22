using System;

namespace MessageBus
{
    /// <summary>
    /// Contains bus settings such as timeouts and message send attempts.
    /// </summary>
    public sealed class BusSettings
    {
        private TimeSpan minRetryTimeout;
        private TimeSpan responseTimeout;
        private TimeSpan sendAttemptTimeout;       
        private byte sendAttempts;

        /// <summary>
        /// Gets or sets a value indicating how long the bus will wait before redelivering an unhandled message.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the timeout is less or equal to 0</exception>
        public TimeSpan MinRetryTimeout
        {
            get
            {
                return minRetryTimeout;
            }
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                minRetryTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating how long the bus will wait for a response.
        /// Implement <see cref="ISaga"/> in the message objects for longer running processes.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the timeout is less or equal to 0</exception>
        public TimeSpan ResponseTimeout
        {
            get
            {
                return responseTimeout;
            }
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                responseTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating how long the bus will wait between the send attempts in case
        /// of a send failure.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the timeout is less or equal to 0</exception>
        public TimeSpan SendAttemptTimeout
        {
            get
            {
                return sendAttemptTimeout;
            }
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                sendAttemptTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of send attempts in case of a failure.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the number of attempts is equal to 0</exception>
        public byte SendAttempts
        {
            get
            {
                return sendAttempts;
            }
            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                sendAttempts = value;
            }
        }
    }
}
