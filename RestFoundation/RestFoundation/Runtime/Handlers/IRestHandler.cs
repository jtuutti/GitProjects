// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System.Web;
using System.Web.Routing;

namespace RestFoundation.Runtime.Handlers
{
    /// <summary>
    /// Defines a REST service handler.
    /// </summary>
    public interface IRestHandler : IRouteHandler, IHttpHandler
    {
        /// <summary>
        /// Gets the service context.
        /// </summary>
        IServiceContext Context { get; }

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
    }
}
