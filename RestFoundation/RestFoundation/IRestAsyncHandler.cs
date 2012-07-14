using System.Web;

namespace RestFoundation
{
    /// <summary>
    /// Defines an asynchronous REST service handler.
    /// </summary>
    public interface IRestAsyncHandler : IRestHandler, IHttpAsyncHandler
    {
    }
}
