// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Web;

namespace RestFoundation.Runtime.Handlers
{
    /// <summary>
    /// Defines a REST service handler.
    /// </summary>
    public interface IRestServiceHandler : IServiceContextHandler, IHttpAsyncHandler
    {
        /// <summary>
        /// Gets a fully qualified name of the interface type defining the service contract.
        /// </summary>
        string ServiceContractTypeName { get; }

        /// <summary>
        /// Gets the service URL.
        /// </summary>
        string ServiceUrl { get; }

        /// <summary>
        /// Gets a relative URL template.
        /// </summary>
        string UrlTemplate { get; }

        /// <summary>
        /// Gets or sets a value representing a timeout for an asynchronous task returned by a
        /// service method. Setting the value to <see cref="TimeSpan.Zero"/> indicates no timeout.
        /// </summary>
        TimeSpan ServiceAsyncTimeout { get; set; }
    }
}
