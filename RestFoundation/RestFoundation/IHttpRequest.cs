// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>

using System;
using System.IO;
using RestFoundation.Collections;
using RestFoundation.Runtime;

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
        /// Gets the dynamic resource object bag.
        /// </summary>
        dynamic ResourceBag { get; }

        /// <summary>
        /// Gets a resource state associated with the service method with associated validation errors.
        /// </summary>
        ResourceState ResourceState { get; }

        /// <summary>
        /// Gets the route collection.
        /// </summary>
        IRouteValueCollection RouteValues { get; }

        /// <summary>
        /// Gets the request header collection.
        /// </summary>
        IHeaderCollection Headers { get; }

        /// <summary>
        /// Gets the query string collection.
        /// </summary>
        IStringValueCollection QueryString { get; }

        /// <summary>
        /// Gets an uploaded file collection.
        /// </summary>
        IUploadedFileCollection Files { get; }

        /// <summary>
        /// Gets the form name/value collection.
        /// </summary>
        IStringValueCollection Form { get; }

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
