// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Specialized;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Web;

namespace RestFoundation.Client
{
    /// <summary>
    /// Defines a REST HTTP client.
    /// </summary>
    public interface IRestClient
    {
        /// <summary>
        /// Gets or sets a connection timeout.
        /// </summary>
        TimeSpan ConnectionTimeout { get; set; }

        /// <summary>
        /// Gets or sets a socket timeout.
        /// </summary>
        TimeSpan SocketTimeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request should perform redirects.
        /// </summary>
        bool PerformRedirects { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request should allow cookies.
        /// </summary>
        bool AllowCookies { get; set; }

        /// <summary>
        /// Gets or sets a proxy URL for tracing HTTP requests and responses.
        /// </summary>
        string ProxyUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the client should support HTTP encoding.
        /// GZIP and Deflate compression algorithms are currently supported.
        /// </summary>
        bool SupportsEncoding { get; set; }

        /// <summary>
        /// Gets or sets the authentication type.
        /// </summary>
        string AuthenticationType { get; set; }

        /// <summary>
        /// Gets or sets authentication credentials.
        /// </summary>
        NetworkCredential Credentials { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether self-signed certificates are allowed.
        /// </summary>
        bool AllowSelfSignedCertificates { get; set; }

        /// <summary>
        /// Gets the last HTTP status code.
        /// </summary>
        int LastStatusCode { get; }

        /// <summary>
        /// Gets the last HTTP status description.
        /// </summary>
        string LastStatusDescription { get; }

                /// <summary>
        /// Executes an HTTP request to the provided URL using the specified HTTP verb.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The HTTP verb.</param>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        void Execute(Uri url, HttpMethod method);

        /// <summary>
        /// Executes an HTTP request to the provided URL using the specified HTTP verb.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The HTTP verb.</param>
        /// <param name="headers">A collection of HTTP headers to pass to the request.</param>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        void Execute(Uri url, HttpMethod method, NameValueCollection headers);

        /// <summary>
        /// Executes an HTTP request to the provided URL using the specified HTTP verb and the provided resource.
        /// </summary>
        /// <typeparam name="TInput">The input resource object type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="method">The HTTP verb.</param>
        /// <param name="resource">The input resource.</param>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        /// <exception cref="ArgumentException">If the resource body is null.</exception>
        void Execute<TInput>(Uri url, HttpMethod method, RestResource<TInput> resource);

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type.
        /// </summary>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="method">The HTTP verb.</param>
        /// <param name="outputType">The output resource type.</param>
        /// <returns>The output resource.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        RestResource<TOutput> Execute<TOutput>(Uri url, HttpMethod method, RestResourceType outputType);

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type.
        /// </summary>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="method">The HTTP verb.</param>
        /// <param name="outputType">The output resource type.</param>
        /// <param name="headers">A collection of HTTP headers to pass to the request.</param>
        /// <returns>The output resource.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        RestResource<TOutput> Execute<TOutput>(Uri url, HttpMethod method, RestResourceType outputType, NameValueCollection headers);

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type.
        /// </summary>
        /// <typeparam name="TInput">The input resource object type.</typeparam>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="method">The HTTP verb.</param>
        /// <param name="resource">The input resource.</param>
        /// <returns>The output resource.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        /// <exception cref="ArgumentException">If the resource body is null.</exception>
        RestResource<TOutput> Execute<TInput, TOutput>(Uri url, HttpMethod method, RestResource<TInput> resource);

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type.
        /// </summary>
        /// <typeparam name="TInput">The input resource object type.</typeparam>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="method">The HTTP verb.</param>
        /// <param name="resource">The input resource.</param>
        /// <param name="outputType">The output resource type.</param>
        /// <returns>The output resource.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        /// <exception cref="ArgumentException">If the resource body is null.</exception>
        RestResource<TOutput> Execute<TInput, TOutput>(Uri url, HttpMethod method, RestResource<TInput> resource, RestResourceType outputType);

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type asynchronously.
        /// </summary>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="method">The HTTP verb.</param>
        /// <param name="outputType">The output resource type.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        Task<RestResource<TOutput>> ExecuteAsync<TOutput>(Uri url, HttpMethod method, RestResourceType outputType);

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type asynchronously.
        /// </summary>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="method">The HTTP verb.</param>
        /// <param name="outputType">The output resource type.</param>
        /// <param name="headers">A collection of HTTP headers to pass to the request.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        Task<RestResource<TOutput>> ExecuteAsync<TOutput>(Uri url, HttpMethod method, RestResourceType outputType, NameValueCollection headers);

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type asynchronously.
        /// </summary>
        /// <typeparam name="TInput">The input resource object type.</typeparam>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="method">The HTTP verb.</param>
        /// <param name="resource">The input resource.</param>
        /// <param name="outputType">The output resource type.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        /// <exception cref="ArgumentException">If the resource body is null.</exception>
        Task<RestResource<TOutput>> ExecuteAsync<TInput, TOutput>(Uri url, HttpMethod method, RestResource<TInput> resource, RestResourceType outputType);
    }
}
