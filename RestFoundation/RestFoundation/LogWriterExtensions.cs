using System;
using System.Globalization;

namespace RestFoundation
{
    /// <summary>
    /// Contains <see cref="ILogWriter"/> extensions.
    /// </summary>
    public static class LogWriterExtensions
    {
        /// <summary>
        /// Writes a debug message into the log buffer if the provided <paramref name="condition"/>
        /// evaluates to true.
        /// </summary>
        /// <param name="writer">The log writer.</param>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="debug">The debug message.</param>
        /// <returns>The log writer instance.</returns>
        public static ILogWriter WriteDebugIf(this ILogWriter writer, bool condition, string debug)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (!condition)
            {
                return writer;
            }

            writer.WriteDebug(debug);
            return writer;
        }

        /// <summary>
        /// Writes an error message into the log buffer if the provided <paramref name="condition"/>
        /// evaluates to true.
        /// </summary>
        /// <param name="writer">The log writer.</param>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="error">The error message.</param>
        /// <returns>The log writer instance.</returns>
        public static ILogWriter WriteErrorIf(this ILogWriter writer, bool condition, string error)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (!condition)
            {
                return writer;
            }

            writer.WriteError(error);
            return writer;
        }

        /// <summary>
        /// Writes an info message into the log buffer if the provided <paramref name="condition"/>
        /// evaluates to true.
        /// </summary>
        /// <param name="writer">The log writer.</param>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="info">The info message.</param>
        /// <returns>The log writer instance.</returns>
        public static ILogWriter WriteInfoIf(this ILogWriter writer, bool condition, string info)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (!condition)
            {
                return writer;
            }

            writer.WriteInfo(info);
            return writer;
        }

        /// <summary>
        /// Writes a warning message into the log buffer if the provided <paramref name="condition"/>
        /// evaluates to true.
        /// </summary>
        /// <param name="writer">The log writer.</param>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="warning">The warning message.</param>
        /// <returns>The log writer instance.</returns>
        public static ILogWriter WriteWarningIf(this ILogWriter writer, bool condition, string warning)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (!condition)
            {
                return writer;
            }

            writer.WriteWarning(warning);
            return writer;
        }

        /// <summary>
        /// Writes a debug message generated from the <paramref name="format"/> string and the provided arguments.
        /// </summary>
        /// <param name="writer">The log writer.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        /// <returns>The log writer instance.</returns>
        /// <exception cref="FormatException">
        /// If the <paramref name="format"/> string does not match the arguments.
        /// </exception>
        public static ILogWriter WriteDebugFormat(this ILogWriter writer, string format, params object[] args)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            writer.WriteDebug(String.Format(CultureInfo.InvariantCulture, format, args));
            return writer;
        }

        /// <summary>
        /// Writes a debug message generated from the <paramref name="format"/> string and the provided arguments.
        /// </summary>
        /// <param name="writer">The log writer.</param>
        /// <param name="provider">The format provider.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        /// <returns>The log writer instance.</returns>
        /// <exception cref="FormatException">
        /// If the <paramref name="format"/> string does not match the arguments.
        /// </exception>
        public static ILogWriter WriteDebugFormat(this ILogWriter writer, IFormatProvider provider, string format, params object[] args)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            writer.WriteDebug(String.Format(provider, format, args));
            return writer;
        }

        /// <summary>
        /// Writes an error message generated from the <paramref name="format"/> string and the provided arguments.
        /// </summary>
        /// <param name="writer">The log writer.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        /// <returns>The log writer instance.</returns>
        /// <exception cref="FormatException">
        /// If the <paramref name="format"/> string does not match the arguments.
        /// </exception>
        public static ILogWriter WriteErrorFormat(this ILogWriter writer, string format, params object[] args)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            writer.WriteError(String.Format(CultureInfo.InvariantCulture, format, args));
            return writer;
        }

        /// <summary>
        /// Writes an error message generated from the <paramref name="format"/> string and the provided arguments.
        /// </summary>
        /// <param name="writer">The log writer.</param>
        /// <param name="provider">The format provider.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        /// <returns>The log writer instance.</returns>
        /// <exception cref="FormatException">
        /// If the <paramref name="format"/> string does not match the arguments.
        /// </exception>
        public static ILogWriter WriteErrorFormat(this ILogWriter writer, IFormatProvider provider, string format, params object[] args)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            writer.WriteError(String.Format(provider, format, args));
            return writer;
        }

        /// <summary>
        /// Writes an info message generated from the <paramref name="format"/> string and the provided arguments.
        /// </summary>
        /// <param name="writer">The log writer.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        /// <returns>The log writer instance.</returns>
        /// <exception cref="FormatException">
        /// If the <paramref name="format"/> string does not match the arguments.
        /// </exception>
        public static ILogWriter WriteInfoFormat(this ILogWriter writer, string format, params object[] args)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            writer.WriteInfo(String.Format(CultureInfo.InvariantCulture, format, args));
            return writer;
        }

        /// <summary>
        /// Writes an info message generated from the <paramref name="format"/> string and the provided arguments.
        /// </summary>
        /// <param name="writer">The log writer.</param>
        /// <param name="provider">The format provider.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        /// <returns>The log writer instance.</returns>
        /// <exception cref="FormatException">
        /// If the <paramref name="format"/> string does not match the arguments.
        /// </exception>
        public static ILogWriter WriteInfoFormat(this ILogWriter writer, IFormatProvider provider, string format, params object[] args)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            writer.WriteInfo(String.Format(provider, format, args));
            return writer;
        }

        /// <summary>
        /// Writes a warning message generated from the <paramref name="format"/> string and the provided arguments.
        /// </summary>
        /// <param name="writer">The log writer.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        /// <returns>The log writer instance.</returns>
        /// <exception cref="FormatException">
        /// If the <paramref name="format"/> string does not match the arguments.
        /// </exception>
        public static ILogWriter WriteWarningFormat(this ILogWriter writer, string format, params object[] args)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            writer.WriteWarning(String.Format(CultureInfo.InvariantCulture, format, args));
            return writer;
        }

        /// <summary>
        /// Writes a warning message generated from the <paramref name="format"/> string and the provided arguments.
        /// </summary>
        /// <param name="writer">The log writer.</param>
        /// <param name="provider">The format provider.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        /// <returns>The log writer instance.</returns>
        /// <exception cref="FormatException">
        /// If the <paramref name="format"/> string does not match the arguments.
        /// </exception>
        public static ILogWriter WriteWarningFormat(this ILogWriter writer, IFormatProvider provider, string format, params object[] args)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            writer.WriteWarning(String.Format(provider, format, args));
            return writer;
        }
    }
}
