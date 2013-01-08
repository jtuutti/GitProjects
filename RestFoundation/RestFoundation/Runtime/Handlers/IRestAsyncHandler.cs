// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System.Web;

namespace RestFoundation.Runtime.Handlers
{
    /// <summary>
    /// Defines an asynchronous REST service handler.
    /// </summary>
    public interface IRestAsyncHandler : IRestHandler, IHttpAsyncHandler
    {
    }
}
