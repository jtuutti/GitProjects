// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Web;
using RestFoundation.Runtime;

namespace RestFoundation
{
    /// <summary>
    /// Defines a service context.
    /// </summary>
    public interface IServiceContext : IRestContext
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
        /// Gets or sets security information for the current HTTP request.
        /// </summary>
        IPrincipal User { get; set; }

        /// <summary>
        /// Gets a value indicating whether there is an authenticated user associated with the HTTP request.
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Gets a value indicating whether the service is running in debug mode.
        /// </summary>
        bool IsDebug { get; }

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        string GetPath<TContract>(Expression<Action<TContract>> serviceMethod);

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        string GetPath<TContract>(Expression<Action<TContract>> serviceMethod, RouteHash routeValues);

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="RestFoundation.Configuration.UrlBuilder"/>
        /// configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        string GetPath<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod);

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="RestFoundation.Configuration.UrlBuilder"/>
        /// configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        string GetPath<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod, RouteHash routeValues);

        /// <summary>
        /// Gets the application absolute URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="RestFoundation.Configuration.UrlBuilder"/>
        /// configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <param name="segments">URI segments necessary to generate an absolute URL. Set it to null to generate a relative URL.</param>
        /// <returns>The application absolute URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        string GetPath<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod, RouteHash routeValues, UriSegments segments);

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        string GetPath<TContract>(Expression<Func<TContract, object>> serviceMethod);
       
        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        string GetPath<TContract>(Expression<Func<TContract, object>> serviceMethod, RouteHash routeValues);

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="RestFoundation.Configuration.UrlBuilder"/>
        /// configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        string GetPath<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod);

        /// <summary>
        /// Gets the application relative URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="RestFoundation.Configuration.UrlBuilder"/>
        /// configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <returns>The application relative URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        string GetPath<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod, RouteHash routeValues);

        /// <summary>
        /// Gets the application absolute URL for a service contract method.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">
        /// The service URL defined by the MapUrl(serviceUrl) function by the <see cref="RestFoundation.Configuration.UrlBuilder"/>
        /// configuration object.
        /// </param>
        /// <param name="serviceMethod">The service contract method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <param name="segments">URI segments necessary to generate an absolute URL. Set it to null to generate a relative URL.</param>
        /// <returns>The application absolute URL for the service method.</returns>
        /// <exception cref="InvalidOperationException">
        /// If an invalid service URL or a service method provided.
        /// </exception>
        string GetPath<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod, RouteHash routeValues, UriSegments segments);

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
