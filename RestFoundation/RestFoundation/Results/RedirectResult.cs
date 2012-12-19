// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Net;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a redirect result. This result is not recommended to use for methods that return JSON or XML.
    /// Set the "Location" HTTP response header instead.
    /// </summary>
    public class RedirectResult : IResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectResult"/> class.
        /// </summary>
        public RedirectResult()
        {
            RedirectType = RedirectType.Found;
        }

        /// <summary>
        /// Gets or sets the redirect type.
        /// </summary>
        public RedirectType RedirectType { get; set; }

        /// <summary>
        /// Gets or sets a redirect URL.
        /// </summary>
        public string RedirectUrl { get; set; }

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

            if (!String.IsNullOrWhiteSpace(RedirectUrl))
            {
                context.Response.SetHeader(context.Response.Headers.Location, RedirectUrl);
                context.Response.SetStatus((HttpStatusCode) (int) RedirectType);
            }
        }
    }
}
