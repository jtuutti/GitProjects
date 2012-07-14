using System;
using System.Collections.Generic;
using System.Net;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents an HTTP response status result.
    /// </summary>
    public class StatusResult : IResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusResult"/> class.
        /// </summary>
        public StatusResult()
        {
            ResponseHeaders = new Dictionary<string, string>();
        }
        
        /// <summary>
        /// Gets a dictionary of response headers to set along with the status code.
        /// </summary>
        public IDictionary<string, string> ResponseHeaders { get; protected set; }

        /// <summary>
        /// Gets or sets an HTTP status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets an HTTP status description.
        /// </summary>
        public string StatusDescription { get; set; }

        /// <summary>
        /// Executes the result against the provided service context.
        /// </summary>
        /// <param name="context">The service context.</param>
        public virtual void Execute(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (StatusCode == HttpStatusCode.NoContent)
            {
                context.Response.Output.Clear();
            }

            foreach (var header in ResponseHeaders)
            {
                context.Response.SetHeader(header.Key, header.Value);
            }

            context.Response.SetStatus(StatusCode, StatusDescription ?? String.Empty);
        }
    }
}
