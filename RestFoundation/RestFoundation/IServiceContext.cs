// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Web;
using System.Web.Routing;

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
        /// Gets the application path for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="UrlBuilder"/> configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <returns>The application path for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or the service method provided.
        /// </exception>
        string GetPath<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod);

        /// <summary>
        /// Gets the application path for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="UrlBuilder"/> configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values based on the object properties.</param>
        /// <returns>The application path for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or the service method provided.
        /// </exception>
        string GetPath<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod, object routeValues);

        /// <summary>
        /// Gets the application path for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="UrlBuilder"/> configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <returns>The application path for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or the service method provided.
        /// </exception>
        string GetPath<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod, RouteValueDictionary routeValues);

        /// <summary>
        /// Gets the application path for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="UrlBuilder"/> configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <returns>The application path for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or the service method provided.
        /// </exception>
        string GetPath<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod);

        /// <summary>
        /// Gets the application path for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="UrlBuilder"/> configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values based on the object properties.</param>
        /// <returns>The application path for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or the service method provided.
        /// </exception>
        string GetPath<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod, object routeValues);

        /// <summary>
        /// Gets the application path for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="UrlBuilder"/> configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <returns>The application path for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or the service method provided.
        /// </exception>
        string GetPath<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod, RouteValueDictionary routeValues);

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
