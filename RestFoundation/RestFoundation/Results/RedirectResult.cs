using System;
using System.Net;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a redirect result. This result is not recommended to use for methods that return JSON or XML.
    /// Set the Location HTTP response header instead.
    /// </summary>
    public class RedirectResult : IResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the redirect is permanent.
        /// </summary>
        public bool IsPermanent { get; set; }

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
            if (context == null) throw new ArgumentNullException("context");

            context.Response.SetHeader(context.Response.Headers.Location, RedirectUrl);
            context.Response.SetStatus(IsPermanent ? HttpStatusCode.MovedPermanently : HttpStatusCode.Redirect);
        }
    }
}
