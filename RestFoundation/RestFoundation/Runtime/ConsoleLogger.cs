// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents a log writer that writes messages into the <see cref="Console"/> output.
    /// </summary>
    public class ConsoleLogWriter : ILogWriter
    {
        /// <summary>
        /// Gets a value indicating whether to log messages automatically generated
        /// by the REST foundation.
        /// </summary>
        public bool LogGeneratedInfo
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Writes a debug message into the log buffer.
        /// </summary>
        /// <param name="debug">A debug message.</param>
        /// <returns>The log writer instance.</returns>
        public ILogWriter WriteDebug(string debug)
        {
            Console.WriteLine(debug);
            return this;
        }

        /// <summary>
        /// Writes an error message into the log buffer.
        /// </summary>
        /// <param name="error">An error message.</param>
        /// <returns>The log writer instance.</returns>
        public ILogWriter WriteError(string error)
        {
            Console.WriteLine(error);
            return this;
        }

        /// <summary>
        /// Writes an information message into the log buffer.
        /// </summary>
        /// <param name="info">An information message.</param>
        /// <returns>The log writer instance.</returns>
        public ILogWriter WriteInfo(string info)
        {
            Console.WriteLine(info);
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
            Console.WriteLine(warning);
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
