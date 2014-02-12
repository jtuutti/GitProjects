// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>

using System;
using System.Threading.Tasks;

namespace RestFoundation
{
    /// <summary>
    /// Defines a log writer for the services.
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        /// Gets a value indicating whether to log messages automatically generated
        /// by the REST foundation.
        /// </summary>
        bool LogGeneratedInfo { get; }

        /// <summary>
        /// Gets a value indicating whether the log writer supports the <see cref="ILogWriter.FlushAsync"/>
        /// operation.
        /// </summary>
        bool SupportsAsyncFlush { get; }

        /// <summary>
        /// Writes a debug message into the log buffer.
        /// The implementation should add a line break at the end of the message.
        /// </summary>
        /// <param name="debug">The debug message.</param>
        /// <returns>The log writer instance.</returns>
        ILogWriter WriteDebug(string debug);

        /// <summary>
        /// Writes an error message into the log buffer.
        /// The implementation should add a line break at the end of the message.
        /// </summary>
        /// <param name="error">The error message.</param>
        /// <returns>The log writer instance.</returns>
        ILogWriter WriteError(string error);

        /// <summary>
        /// Writes an information message into the log buffer.
        /// The implementation should add a line break at the end of the message.
        /// </summary>
        /// <param name="info">The information message.</param>
        /// <returns>The log writer instance.</returns>
        ILogWriter WriteInfo(string info);

        /// <summary>
        /// Writes a warning message into the log buffer.
        /// The implementation should add a line break at the end of the message.
        /// </summary>
        /// <param name="warning">The warning message.</param>
        /// <returns>The log writer instance.</returns>
        ILogWriter WriteWarning(string warning);

        /// <summary>
        /// Flushes the log buffer.
        /// </summary>
        void Flush();

        /// <summary>
        /// Flushes the log buffer asynchronously. The synchronous <see cref="ILogWriter.Flush"/>
        /// method must also be implemented for the REST Foundation HTTP module to work correctly.
        /// </summary>
        /// <returns>The task that flashes the log buffer.</returns>
        /// <exception cref="NotSupportedException">
        /// If the logger's <see cref="SupportsAsyncFlush"/> property is set to false.
        /// </exception>
        Task FlushAsync();
    }
}
