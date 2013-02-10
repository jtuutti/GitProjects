// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>

using System;
using System.Threading.Tasks;
using System.Web;

namespace RestFoundation.Runtime.Handlers
{
    /// <summary>
    /// Defines a REST service handler.
    /// </summary>
    public interface IRestServiceHandler : IServiceContextHandler, IHttpAsyncHandler
    {
        /// <summary>
        /// Gets the service URL.
        /// </summary>
        string ServiceUrl { get; }

        /// <summary>
        /// Gets a fully qualified name of the interface type defining the service contract.
        /// </summary>
        string ServiceContractTypeName { get; }

        /// <summary>
        /// Gets a relative URL template.
        /// </summary>
        string UrlTemplate { get; }

        /// <summary>
        /// Gets or sets a value representing the service method execution timeout.
        /// Setting the value to <see cref="TimeSpan.Zero"/> indicates no timeout.
        /// </summary>
        TimeSpan ServiceTimeout { get; set; }

        /// <summary>
        /// Gets or sets a value representing the service method result execution timeout.
        /// Setting the value to <see cref="TimeSpan.Zero"/> indicates no timeout.
        /// </summary>
        TimeSpan ResultTimeout { get; set; }

        /// <summary>
        /// When overridden in a derived class, provides code that handles an asynchronous task.
        /// </summary>
        /// <returns>The asynchronous task.</returns>
        /// <param name="context">The HTTP context.</param>
        Task ProcessRequestAsync(HttpContext context);
    }
}
