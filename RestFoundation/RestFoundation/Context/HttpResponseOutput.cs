// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.IO;
using System.Threading.Tasks;

namespace RestFoundation.Context
{
    /// <summary>
    /// Represents the output of an HTTP response.
    /// </summary>
    public class HttpResponseOutput : ContextBase, IHttpResponseOutput
    {
        private const string LineBreak = "<br/>";

        private readonly ILogWriter m_logWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseOutput"/> class.
        /// </summary>
        /// <param name="logWriter">The log writer.</param>
        public HttpResponseOutput(ILogWriter logWriter)
        {
            m_logWriter = logWriter;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the output should be buffered.
        /// </summary>
        public bool Buffer
        {
            get
            {
                return Context.Response.BufferOutput;
            }
            set
            {
                Context.Response.BufferOutput = value;
            }
        }

        /// <summary>
        /// Gets the output stream.
        /// </summary>
        public Stream Stream
        {
            get
            {
                return Context.Response.OutputStream;
            }
        }

        /// <summary>
        /// Gets the output writer.
        /// </summary>
        public TextWriter Writer
        {
            get
            {
                return Context.Response.Output;
            }
        }

        /// <summary>
        /// Gets the log writer.
        /// </summary>
        public ILogWriter LogWriter
        {
            get
            {
                return m_logWriter;
            }
        }

        /// <summary>
        /// Gets or sets a filter stream that modifies the output during the data transmission.
        /// </summary>
        public Stream Filter
        {
            get
            {
                return Context.Response.Filter;
            }
            set
            {
                Context.Response.Filter = value;
            }
        }

        /// <summary>
        /// Sends all the buffered output the the client.
        /// </summary>
        public void Flush()
        {
            Context.Response.Flush();
        }

        /// <summary>
        /// Sends all the buffered output the the client asynchronously.
        /// </summary>
        /// <returns>The task that flushes the context.</returns>
        public Task FlushAsync()
        {
            return Task.Factory.FromAsync(Context.Response.BeginFlush, Context.Response.EndFlush, Context);
        }

        /// <summary>
        /// Clears all the output data in the response.
        /// </summary>
        public void Clear()
        {
            Context.Response.Clear();
        }

        /// <summary>
        /// Writes a string into the output stream.
        /// </summary>
        /// <param name="value">A string value to write.</param>
        /// <returns>The response output object.</returns>
        public IHttpResponseOutput Write(string value)
        {
            Writer.Write(value);
            return this;
        }

        /// <summary>
        /// Writes a value into the output stream.
        /// </summary>
        /// <param name="obj">A value to write.</param>
        /// <returns>The response output object.</returns>
        public IHttpResponseOutput Write(object obj)
        {
            Writer.Write(obj);
            return this;
        }

        /// <summary>
        /// Writes a new line into the output stream.
        /// </summary>
        /// <returns>The response output object.</returns>
        public IHttpResponseOutput WriteLine()
        {
            Writer.Write(LineBreak);
            return this;
        }

        /// <summary>
        /// Writes a string followed by a line into the output stream.
        /// </summary>
        /// <param name="value">A string value to write.</param>
        /// <returns>The response output object.</returns>
        public IHttpResponseOutput WriteLine(string value)
        {
            Writer.Write(value);
            Writer.Write(LineBreak);
            return this;
        }

        /// <summary>
        /// Writes the provided number of new lines into the output stream.
        /// </summary>
        /// <param name="times">The number of new lines to write.</param>
        /// <returns>The response output object.</returns>
        public IHttpResponseOutput WriteLine(byte times)
        {
            for (byte i = 0; i < times; i++)
            {
                WriteLine();
            }

            return this;
        }

        /// <summary>
        /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array,
        /// then writes the formatted string into the output stream.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="values">An object array that contains zero or more objects to format.</param>
        /// <returns>The response output object.</returns>
        public IHttpResponseOutput WriteFormat(string format, params object[] values)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }

            Writer.Write(format, values);
            return this;
        }

        /// <summary>
        /// Writes a string to the output stream asynchronously.
        /// </summary>
        /// <param name="value">A string value to write.</param>
        /// <returns>The task that writes the value into the output stream.</returns>
        public Task WriteAsync(string value)
        {
            return Writer.WriteAsync(value);
        }

        /// <summary>
        /// Writes an array of characters to the output stream asynchronously.
        /// </summary>
        /// <param name="buffer">An array of characters.</param>
        /// <param name="index">A position in the array to start writing characters.</param>
        /// <param name="count">A number of characters to write.</param>
        /// <returns>The task that writes the value into the output stream.</returns>
        public Task WriteAsync(char[] buffer, int index, int count)
        {
            return Writer.WriteAsync(buffer, index, count);
        }
    }
}
