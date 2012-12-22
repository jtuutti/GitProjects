// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.IO;

namespace RestFoundation
{
    /// <summary>
    /// Defines an HTTP response output.
    /// </summary>
    public interface IHttpResponseOutput
    {
        /// <summary>
        /// Gets or sets a value indicating whether the output should be buffered.
        /// </summary>
        bool Buffer { get; set; }

        /// <summary>
        /// Gets the output stream.
        /// </summary>
        Stream Stream { get; }

        /// <summary>
        /// Gets the output writer.
        /// </summary>
        TextWriter Writer { get; }

        /// <summary>
        /// Gets or sets a filter stream that modifies the output during the data transmission.
        /// </summary>
        Stream Filter { get; set; }

        /// <summary>
        /// Sends all the buffered output the the client.
        /// </summary>
        void Flush();

        /// <summary>
        /// Clears all the output data in the response.
        /// </summary>
        void Clear();

        /// <summary>
        /// Writes a string into the output stream.
        /// </summary>
        /// <param name="value">A string value to write.</param>
        /// <returns>The response output object.</returns>
        IHttpResponseOutput Write(string value);

        /// <summary>
        /// Writes a value into the output stream.
        /// </summary>
        /// <param name="obj">A value to write.</param>
        /// <returns>The response output object.</returns>
        IHttpResponseOutput Write(object obj);

        /// <summary>
        /// Writes a new line into the output stream.
        /// </summary>
        /// <returns>The response output object.</returns>
        IHttpResponseOutput WriteLine();

        /// <summary>
        /// Writes a string followed by a line into the output stream.
        /// </summary>
        /// <param name="value">A string value to write.</param>
        /// <returns>The response output object.</returns>
        IHttpResponseOutput WriteLine(string value);

        /// <summary>
        /// Writes the provided number of new lines into the output stream.
        /// </summary>
        /// <param name="times">The number of new lines to write.</param>
        /// <returns>The response output object.</returns>
        IHttpResponseOutput WriteLine(byte times);

        /// <summary>
        /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array,
        /// then writes the formatted string into the output stream.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="values">An object array that contains zero or more objects to format.</param>
        /// <returns>The response output object.</returns>
        IHttpResponseOutput WriteFormat(string format, params object[] values);
    }
}
