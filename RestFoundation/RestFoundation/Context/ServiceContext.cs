using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using RestFoundation.Collections.Specialized;

namespace RestFoundation.Context
{
    /// <summary>
    /// Represents a service context.
    /// </summary>
    public class ServiceContext : ContextBase, IServiceContext
    {
        private readonly IHttpRequest m_request;
        private readonly IHttpResponse m_response;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceContext"/> class.
        /// </summary>
        /// <param name="request">The current HTTP request.</param>
        /// <param name="response">The current HTTP response.</param>
        public ServiceContext(IHttpRequest request, IHttpResponse response)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (response == null) throw new ArgumentNullException("response");

            m_request = request;
            m_response = response;
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
        /// Gets the dynamic HTTP context item dictionary.
        /// </summary>
        public dynamic HttpItemBag
        {
            get
            {
                return new DynamicDictionary(Context.Items);
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
