namespace RestFoundation
{
    /// <summary>
    /// Defines a log writer for the services.
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        /// Writes a debug message into the log buffer.
        /// The implementation should add a line break at the end of the message.
        /// </summary>
        /// <param name="debug">A debug message.</param>
        /// <returns>The log writer instance.</returns>
        ILogWriter WriteDebug(string debug);

        /// <summary>
        /// Writes an error message into the log buffer.
        /// The implementation should add a line break at the end of the message.
        /// </summary>
        /// <param name="error">An error message.</param>
        /// <returns>The log writer instance.</returns>
        ILogWriter WriteError(string error);

        /// <summary>
        /// Writes an information message into the log buffer.
        /// The implementation should add a line break at the end of the message.
        /// </summary>
        /// <param name="info">An information message.</param>
        /// <returns>The log writer instance.</returns>
        ILogWriter WriteInfo(string info);

        /// <summary>
        /// Flushes the log buffer.
        /// </summary>
        void Flush();
    }
}
