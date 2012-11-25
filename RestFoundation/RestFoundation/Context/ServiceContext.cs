// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Security.Principal;
using System.Threading;
using System.Web;

namespace RestFoundation.Context
{
    /// <summary>
    /// Represents a service context.
    /// </summary>
    public class ServiceContext : ContextBase, IServiceContext
    {
        private readonly IHttpRequest m_request;
        private readonly IHttpResponse m_response;
        private readonly IServiceCache m_cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceContext"/> class.
        /// </summary>
        /// <param name="request">The current HTTP request.</param>
        /// <param name="response">The current HTTP response.</param>
        /// <param name="cache">The service cache.</param>
        public ServiceContext(IHttpRequest request, IHttpResponse response, IServiceCache cache)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            m_request = request;
            m_response = response;
            m_cache = cache;
        }

        /// <summary>
        /// Gets the current HTTP request.
        /// </summary>
        public IHttpRequest Request
        {
            get
            {
                return m_request;
            }
        }

        /// <summary>
        /// Gets the current HTTP response.
        /// </summary>
        public IHttpResponse Response
        {
            get
            {
                return m_response;
            }
        }

        /// <summary>
        /// Gets the service cache.
        /// </summary>
        public IServiceCache Cache
        {
            get
            {
                return m_cache;
            }
        }

        /// <summary>
        /// Gets or sets a time span before a service times out.
        /// </summary>
        public TimeSpan ServiceTimeout
        {
            get
            {
                return TimeSpan.FromSeconds(Context.Server.ScriptTimeout);
            }
            set
            {
                Context.Server.ScriptTimeout = Convert.ToInt32(value.TotalSeconds);
            }
        }

        /// <summary>
        /// Gets or sets security information for the current HTTP request.
        /// </summary>
        public IPrincipal User
        {
            get
            {
                return Context.User;
            }
            set
            {
                Context.User = value;
                Thread.CurrentPrincipal = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is an authenticated user associated with the HTTP request.
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                return Context.User != null && Context.User.Identity.IsAuthenticated;
            }
        }

        /// <summary>
        /// Returns the physical path associated to the virtual path of the file.
        /// </summary>
        /// <param name="relativePath">The relative virtual path to the file.</param>
        /// <returns>The physical file path.</returns>
        public string MapPath(string relativePath)
        {
            return Context.Server.MapPath(relativePath);
        }

        /// <summary>
        /// Gets the underlying <see cref="HttpContextBase"/> instance that may provide additional functionality.
        /// </summary>
        /// <returns>The underlying HTTP context instance.</returns>
        public HttpContextBase GetHttpContext()
        {
            return Context;
        }
    }
}
