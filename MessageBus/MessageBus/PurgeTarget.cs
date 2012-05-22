using System;

namespace MessageBus
{
    /// <summary>
    /// Defines purge targets for the message bus.
    /// </summary>
    [Flags]
    public enum PurgeTargets
    {
        /// <summary>
        /// No messages
        /// </summary>
        None = 0,

        /// <summary>
        /// Input messages
        /// </summary>
        InputMessages = 1,

        /// <summary>
        /// Pending messages
        /// </summary>
        PendingMessages = 2,

        /// <summary>
        /// Fault messages
        /// </summary>
        FaultMessages = 4,

        /// <summary>
        /// Responses to pending messages
        /// </summary>
        ResponseMessages = 8,

        /// <summary>
        /// All messages except faults
        /// </summary>
        NonFaultMessages = InputMessages | PendingMessages | ResponseMessages,

        /// <summary>
        /// All messages
        /// </summary>
        AllMessages = InputMessages | PendingMessages | FaultMessages | ResponseMessages
    }
}
