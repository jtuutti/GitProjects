// <copyright>
// Dmitry Starosta, 2012
// </copyright>
namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents a dummy log writer that ignores logged messages.
    /// </summary>
    public sealed class NullLogWriter : ILogWriter
    {
        /// <summary>
        /// Writes a debug message into the log buffer.
        /// </summary>
        /// <param name="debug">A debug message.</param>
        /// <returns>The log writer instance.</returns>
        public ILogWriter WriteDebug(string debug)
        {
            return this;
        }

        /// <summary>
        /// Writes an information message into the log buffer.
        /// </summary>
        /// <param name="info">An information message.</param>
        /// <returns>The log writer instance.</returns>
        public ILogWriter WriteInfo(string info)
        {
            return this;
        }

        /// <summary>
        /// Writes an error message into the log buffer.
        /// </summary>
        /// <param name="error">An error message.</param>
        /// <returns>The log writer instance.</returns>
        public ILogWriter WriteError(string error)
        {
            return this;
        }

        /// <summary>
        /// Flushes the log buffer.
        /// </summary>
        public void Flush()
        {
        }
    }
}
