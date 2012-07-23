using System;
using System.Globalization;
using System.IO;

namespace RestFoundation.Context
{
    /// <summary>
    /// Represents the output of an HTTP response.
    /// </summary>
    public class HttpResponseOutput : ContextBase, IHttpResponseOutput
    {
        private const string LineBreak = "<br/>";

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
            if (format == null) throw new ArgumentNullException("format");

            Writer.Write(String.Format(CultureInfo.InvariantCulture, format, values));
            return this;
        }

        /// <summary>
        /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array,
        /// then writes the formatted string into the output stream.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">The format string.</param>
        /// <param name="values">An object array that contains zero or more objects to format.</param>
        /// <returns>The response output object.</returns>
        public IHttpResponseOutput WriteFormat(IFormatProvider provider, string format, params object[] values)
        {
            if (format == null) throw new ArgumentNullException("format");

            Writer.Write(String.Format(provider, format, values));
            return this;
        }
    }
}
