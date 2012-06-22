namespace RestFoundation
{
    /// <summary>
    /// Defines a logger abstraction.
    /// </summary>
    public interface ILogger
    {      
        /// <summary>
        /// Gets or sets a <see cref="bool"/> value indicating whether the FATAL level logging is enabled.
        /// </summary>
        bool IsFatalEnabled { get; }

        /// <summary>
        /// Gets or sets a <see cref="bool"/> value indicating whether the ERROR level logging is enabled.
        /// </summary>
        bool IsErrorEnabled { get; }

        /// <summary>
        /// Gets or sets a <see cref="bool"/> value indicating whether the WARN level logging is enabled.
        /// </summary>
        bool IsWarnEnabled { get; }

        /// <summary>
        /// Gets or sets a <see cref="bool"/> value indicating whether the DEBUG level logging is enabled.
        /// </summary>
        bool IsDebugEnabled { get; }

        /// <summary>
        /// Gets or sets a <see cref="bool"/> value indicating whether the INFO level logging is enabled.
        /// </summary>
        bool IsInfoEnabled { get; }

        /// <summary>
        /// Logs fatal error data.
        /// </summary>
        /// <param name="info">An object to log.</param>
        void Fatal(object info);

        /// <summary>
        /// Logs error data.
        /// </summary>
        /// <param name="info">An object to log.</param>
        void Error(object info);

        /// <summary>
        /// Logs warning data.
        /// </summary>
        /// <param name="info">An object to log.</param>
        void Warn(object info);

        /// <summary>
        /// Logs debugging data.
        /// </summary>
        /// <param name="info">An object to log.</param>
        void Debug(object info);

        /// <summary>
        /// Logs information data.
        /// </summary>
        /// <param name="info">An object to log.</param>
        void Info(object info);

        /// <summary>
        /// Logs fatal error information.
        /// </summary>
        /// <param name="info">A format string to log.</param>
        /// <param name="args">A sequence of parameters.</param>
        void FatalFormat(string info, params object[] args);

        /// <summary>
        /// Logs error information.
        /// </summary>
        /// <param name="info">A format string to log.</param>
        /// <param name="args">A sequence of parameters.</param>
        void ErrorFormat(string info, params object[] args);

        /// <summary>
        /// Logs warning information.
        /// </summary>
        /// <param name="info">A format string to log.</param>
        /// <param name="args">A sequence of parameters.</param>
        void WarnFormat(string info, params object[] args);

        /// <summary>
        /// Logs debugging information.
        /// </summary>
        /// <param name="info">A format string to log.</param>
        /// <param name="args">A sequence of parameters.</param>
        void DebugFormat(string info, params object[] args);

        /// <summary>
        /// Logs information information.
        /// </summary>
        /// <param name="info">A format string to log.</param>
        /// <param name="args">A sequence of parameters.</param>
        void InfoFormat(string info, params object[] args);
    }
}
