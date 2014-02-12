// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System.Web;
using System.Web.Routing;

namespace RestFoundation.Runtime.Handlers
{
    /// <summary>
    /// Defines a service context dependent handler.
    /// </summary>
    public interface IServiceContextHandler : IRouteHandler, IHttpHandler
    {
        /// <summary>
        /// Gets the service context.
        /// </summary>
        IServiceContext Context { get; }
    }
}
