// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Collections.Generic;
using System.Net;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents an HTTP status code result.
    /// </summary>
    public class StatusCodeResult : IResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusCodeResult"/> class.
        /// </summary>
        public StatusCodeResult()
        {
            ResponseHeaders = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusCodeResult"/> class
        /// with the provided HTTP status code.
        /// </summary>
        /// <param name="code">The HTTP status code.</param>
        public StatusCodeResult(HttpStatusCode code) : this()
        {
            Code = code;
        }
        
        /// <summary>
        /// Gets a dictionary of response headers to set along with the status code.
        /// </summary>
        public IDictionary<string, string> ResponseHeaders { get; protected set; }

        /// <summary>
        /// Gets or sets an HTTP status code.
        /// </summary>
        public HttpStatusCode Code { get; set; }

        /// <summary>
        /// Gets or sets an HTTP status description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Executes the result against the provided service context.
        /// </summary>
        /// <param name="context">The service context.</param>
        public virtual void Execute(IServiceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (Code == HttpStatusCode.NoContent)
            {
                context.Response.Output.Clear();
            }

            foreach (var header in ResponseHeaders)
            {
                context.Response.SetHeader(header.Key, header.Value);
            }

            if (!String.IsNullOrWhiteSpace(Description))
            {
                context.Response.SetStatus(Code, Description);
            }
            else
            {
                context.Response.SetStatus(Code);
            }
        }

        /// <summary>
        /// Gets a <see cref="bool"/> indicating whether the status code was successful.
        /// </summary>
        /// <returns>
        /// true if the status code is in the 200-299 range; otherwise false.
        /// </returns>
        public bool IsSuccessStatusCode()
        {
            var numericCode = (int) Code;

            return numericCode >= 200 && numericCode < 300;
        }
    }
}
