// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System.IO;
using RestFoundation.Collections;

namespace RestFoundation
{
    /// <summary>
    /// Defines an HTTP request.
    /// </summary>
    public interface IHttpRequest
    {
        /// <summary>
        /// Gets a value indicating whether the request was initiated through AJAX.
        /// </summary>
        bool IsAjax { get; }

        /// <summary>
        /// Gets a value indicating whether the request was initiated from the local computer.
        /// </summary>
        bool IsLocal { get; }

        /// <summary>
        /// Gets a value indicating whether the HTTP connection uses SSL (HTTPS protocol).
        /// </summary>
        bool IsSecure { get; }

        /// <summary>
        /// Gets the service operation URL.
        /// </summary>
        ServiceOperationUri Url { get; }

        /// <summary>
        /// Gets the HTTP method of the request.
        /// </summary>
        HttpMethod Method { get; }

        /// <summary>
        /// Gets the stream containing the HTTP request body data.
        /// </summary>
        Stream Body { get; }

        /// <summary>
        /// Gets the dynamic query string dictionary.
        /// </summary>
        dynamic QueryBag { get; }

        /// <summary>
        /// Gets the route collection.
        /// </summary>
        IObjectValueCollection RouteValues { get; }

        /// <summary>
        /// Gets the request header collection.
        /// </summary>
        IHeaderCollection Headers { get; }

        /// <summary>
        /// Gets the query string collection.
        /// </summary>
        IStringValueCollection QueryString { get; }

        /// <summary>
        /// Gets the server variable collection.
        /// </summary>
        IServerVariableCollection ServerVariables { get; }

        /// <summary>
        /// Gets the request cookie collection.
        /// </summary>
        ICookieValueCollection Cookies { get; }
    }
}
