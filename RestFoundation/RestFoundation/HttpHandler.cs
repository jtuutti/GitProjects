// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using RestFoundation.Results;
using RestFoundation.Runtime;

namespace RestFoundation
{
    /// <summary>
    /// Represents a base class for an HTTP handler implementation. HTTP handlers are designed for non-RESTful
    /// operations. This class cannot be instantiated.
    /// </summary>
    public abstract class HttpHandler : HttpTaskAsyncHandler, IRouteHandler
    {
        /// <summary>
        /// When overridden in a derived class, provides code that performs the HTTP handler operations.
        /// </summary>
        /// <returns>The asynchronous task containing the <see cref="IResult"/> object.</returns>
        /// <param name="context">The service context.</param>
        public abstract Task<IResult> ExecuteAsync(IServiceContext context);

        /// <summary>
        /// When overridden in a derived class, provides code that handles an asynchronous task.
        /// </summary>
        /// <returns>
        /// The asynchronous task.
        /// </returns>
        /// <param name="context">The HTTP context.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public sealed override async Task ProcessRequestAsync(HttpContext context)
        {
            var serviceContext = Rest.Configuration.ServiceLocator.GetService<IServiceContext>();

            IResult result = await ExecuteAsync(serviceContext);

            if (result == null)
            {
                return;
            }

            var resultExecutor = new ResultExecutor();
            await resultExecutor.Execute(result, serviceContext);
        }

        /// <summary>
        /// When overridden in a derived class, provides code that handles a synchronous task.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The method is implemented but does not provide any default handling for synchronous tasks.
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public sealed override void ProcessRequest(HttpContext context)
        {
            base.ProcessRequest(context);
        }

        /// <summary>
        /// Provides the object that processes the request.
        /// </summary>
        /// <returns>An object that processes the request.</returns>
        /// <param name="requestContext">An object that encapsulates information about the request.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }
    }
}
