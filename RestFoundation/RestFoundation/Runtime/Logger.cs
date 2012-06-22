namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents the default logger dummy implementation that does not do anything.
    /// </summary>
    public class Logger : ILogger
    {
        /// <summary>
        /// Gets or sets a <see cref="bool"/> value indicating whether the FATAL level logging is enabled.
        /// </summary>
        public bool IsFatalEnabled
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="bool"/> value indicating whether the ERROR level logging is enabled.
        /// </summary>
        public bool IsErrorEnabled
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="bool"/> value indicating whether the WARN level logging is enabled.
        /// </summary>
        public bool IsWarnEnabled
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="bool"/> value indicating whether the DEBUG level logging is enabled.
        /// </summary>
        public bool IsDebugEnabled
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="bool"/> value indicating whether the INFO level logging is enabled.
        /// </summary>
        public bool IsInfoEnabled
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Logs fatal error data.
        /// </summary>
        /// <param name="info">An object to log.</param>
        public void Fatal(object info)
        {
        }

        /// <summary>
        /// Logs error data.
        /// </summary>
        /// <param name="info">An object to log.</param>
        public void Error(object info)
        {
        }

        /// <summary>
        /// Logs warning data.
        /// </summary>
        /// <param name="info">An object to log.</param>
        public void Warn(object info)
        {
        }

        /// <summary>
        /// Logs debugging data.
        /// </summary>
        /// <param name="info">An object to log.</param>
        public void Debug(object info)
        {
        }

        /// <summary>
        /// Logs information data.
        /// </summary>
        /// <param name="info">An object to log.</param>
        public void Info(object info)
        {
        }

        /// <summary>
        /// Logs fatal error information.
        /// </summary>
        /// <param name="info">A format string to log.</param>
        /// <param name="args">A sequence of parameters.</param>
        public void FatalFormat(string info, params object[] args)
        {
        }

        /// <summary>
        /// Logs error information.
        /// </summary>
        /// <param name="info">A format string to log.</param>
        /// <param name="args">A sequence of parameters.</param>
        public void ErrorFormat(string info, params object[] args)
        {
        }

        /// <summary>
        /// Logs warning information.
        /// </summary>
        /// <param name="info">A format string to log.</param>
        /// <param name="args">A sequence of parameters.</param>
        public void WarnFormat(string info, params object[] args)
        {
        }

        /// <summary>
        /// Logs debugging information.
        /// </summary>
        /// <param name="info">A format string to log.</param>
        /// <param name="args">A sequence of parameters.</param>
        public void DebugFormat(string info, params object[] args)
        {
        }

        /// <summary>
        /// Logs information information.
        /// </summary>
        /// <param name="info">A format string to log.</param>
        /// <param name="args">A sequence of parameters.</param>
        public void InfoFormat(string info, params object[] args)
        {
        }
    }
}
