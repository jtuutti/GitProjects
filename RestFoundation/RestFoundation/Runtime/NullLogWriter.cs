// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>

using System;
using System.Threading.Tasks;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents a dummy log writer that ignores messages.
    /// </summary>
    public sealed class NullLogWriter : ILogWriter
    {
        /// <summary>
        /// Gets a value indicating whether to log messages automatically generated
        /// by the REST foundation.
        /// </summary>
        public bool LogGeneratedInfo
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the log writer supports the <see cref="ILogWriter.FlushAsync"/>
        /// operation.
        /// </summary>
        public bool SupportsAsyncFlush
        {
            get
            {
                return false;
            }
        }

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
        /// Writes an error message into the log buffer.
        /// </summary>
        /// <param name="error">An error message.</param>
        /// <returns>The log writer instance.</returns>
        public ILogWriter WriteError(string error)
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
        /// Writes a warning message into the log buffer.
        /// The implementation should add a line break at the end of the message.
        /// </summary>
        /// <param name="warning">A warning message.</param>
        /// <returns>The log writer instance.</returns>
        public ILogWriter WriteWarning(string warning)
        {
            return this;
        }

        /// <summary>
        /// Flushes the log buffer.
        /// </summary>
        public void Flush()
        {
        }

        /// <summary>
        /// Flushes the log buffer asynchronously.
        /// </summary>
        /// <returns>The task that flashes the log buffer.</returns>
        /// <exception cref="NotSupportedException">
        /// If the logger's <see cref="ILogWriter.SupportsAsyncFlush"/> property is set to false.
        /// </exception>
        public Task FlushAsync()
        {
            throw new NotSupportedException(Resources.Global.UnsupportedSyncExecutionForAsyncResult);
        }
    }
}
