// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
namespace RestFoundation
{
    /// <summary>
    /// Defines the detail of information returned in the fault collection object during an
    /// unhandled service exception.
    /// </summary>
    public enum FaultDetail
    {
        /// <summary>
        /// Only the exception message is returned.
        /// </summary>
        MessageOnly,

        /// <summary>
        /// The exception message and the stack trace are returned in the debug mode.
        /// Only the exception message is returned in the release mode.
        /// This is the default setting.
        /// </summary>
        DetailedInDebugMode,

        /// <summary>
        /// The exception message and the stack trace are always returned.
        /// </summary>
        Detailed
    }
}
