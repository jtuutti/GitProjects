// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Security.Principal;
using System.Web;

namespace RestFoundation
{
    /// <summary>
    /// Defines a service context.
    /// </summary>
    public interface IServiceContext
    {
        /// <summary>
        /// Gets the current HTTP request.
        /// </summary>
        IHttpRequest Request { get; }

        /// <summary>
        /// Gets the current HTTP response.
        /// </summary>
        IHttpResponse Response { get; }

        /// <summary>
        /// Gets the service cache.
        /// </summary>
        IServiceCache Cache { get; }

        /// <summary>
        /// Gets or sets a time span before a service times out.
        /// </summary>
        TimeSpan ServiceTimeout { get; set; }

        /// <summary>
        /// Gets or sets security information for the current HTTP request.
        /// </summary>
        IPrincipal User { get; set; }

        /// <summary>
        /// Gets a value indicating whether there is an authenticated user associated with the HTTP request.
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Gets the dynamic HTTP context item dictionary.
        /// </summary>
        dynamic HttpItemBag { get; }

        /// <summary>
        /// Returns the physical path associated to the virtual path of the file.
        /// </summary>
        /// <param name="relativePath">The relative virtual path to the file.</param>
        /// <returns>The physical file path.</returns>
        string MapPath(string relativePath);

        /// <summary>
        /// Gets the underlying <see cref="HttpContextBase"/> instance that may provide additional functionality.
        /// </summary>
        /// <returns>The underlying HTTP context instance.</returns>
        HttpContextBase GetHttpContext();
    }
}
