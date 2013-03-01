// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
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
        /// Gets or sets a value indicating whether the request should perform redirects.
        /// </summary>
        bool PerformRedirects { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request should allow cookies.
        /// </summary>
        bool AllowCookies { get; set; }

        /// <summary>
        /// Gets or sets a proxy URL for inspecting HTTP requests and responses.
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
        /// Executes an HTTP request to the provided URL using the GET HTTP method.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>A task resulting in a collection of HTTP headers returned.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        Task<WebHeaderCollection> GetAsync(Uri url);

        /// <summary>
        /// Executes an HTTP request to the provided URL using the GET HTTP method.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="headers">A collection of HTTP headers to pass to the request.</param>
        /// <returns>A task resulting in a collection of HTTP headers returned.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        Task<WebHeaderCollection> GetAsync(Uri url, NameValueCollection headers);

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type using the GET HTTP method.
        /// </summary>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="outputType">The output resource type.</param>
        /// <returns>A task resulting in an output resource.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        Task<RestResource<TOutput>> GetAsync<TOutput>(Uri url, RestResourceType outputType);

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type using the GET HTTP method.
        /// </summary>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="outputType">The output resource type.</param>
        /// <param name="headers">A collection of HTTP headers to pass to the request.</param>
        /// <returns>A task resulting in an output resource.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        Task<RestResource<TOutput>> GetAsync<TOutput>(Uri url, RestResourceType outputType, NameValueCollection headers);

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type using the GET HTTP method.
        /// </summary>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="outputType">The output resource type.</param>
        /// <param name="headers">A collection of HTTP headers to pass to the request.</param>
        /// <param name="xmlNamespace">An optional XML namespace.</param>
        /// <returns>A task resulting in an output resource.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        Task<RestResource<TOutput>> GetAsync<TOutput>(Uri url, RestResourceType outputType, NameValueCollection headers, string xmlNamespace);

        /// <summary>
        /// Executes an HTTP request to the provided URL using the HEAD HTTP method.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>A task resulting in a collection of HTTP headers returned.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        Task<WebHeaderCollection> HeadAsync(Uri url);

        /// <summary>
        /// Executes an HTTP request to the provided URL using the HEAD HTTP method.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="headers">A collection of HTTP headers to pass to the request.</param>
        /// <returns>A task resulting in a collection of HTTP headers returned.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        Task<WebHeaderCollection> HeadAsync(Uri url, NameValueCollection headers);

        /// <summary>
        /// Executes an HTTP request to the provided URL using the HEAD HTTP method.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="outputType">The output resource type.</param>
        /// <returns>A task resulting in a collection of HTTP headers returned.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        Task<WebHeaderCollection> HeadAsync(Uri url, RestResourceType outputType);

        /// <summary>
        /// Executes an HTTP request to the provided URL using the HEAD HTTP method.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="outputType">The output resource type.</param>
        /// <param name="headers">A collection of HTTP headers to pass to the request.</param>
        /// <returns>A task resulting in a collection of HTTP headers returned.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        Task<WebHeaderCollection> HeadAsync(Uri url, RestResourceType outputType, NameValueCollection headers);

        /// <summary>
        /// Executes an HTTP request to the provided URL using the HEAD HTTP method.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="outputType">The output resource type.</param>
        /// <param name="headers">A collection of HTTP headers to pass to the request.</param>
        /// <param name="xmlNamespace">An optional XML namespace.</param>
        /// <returns>A task resulting in a collection of HTTP headers returned.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        Task<WebHeaderCollection> HeadAsync(Uri url, RestResourceType outputType, NameValueCollection headers, string xmlNamespace);

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type using the POST HTTP method.
        /// </summary>
        /// <typeparam name="TInput">The input resource object type.</typeparam>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="resource">The input resource.</param>
        /// <returns>A task resulting in an output resource.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        /// <exception cref="ArgumentException">If the resource body is null.</exception>
        Task<RestResource<TOutput>> PostAsync<TInput, TOutput>(Uri url, RestResource<TInput> resource);

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type using the POST HTTP method.
        /// </summary>
        /// <typeparam name="TInput">The input resource object type.</typeparam>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="resource">The input resource.</param>
        /// <param name="outputType">The output resource type.</param>
        /// <returns>A task resulting in an output resource.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        /// <exception cref="ArgumentException">If the resource body is null.</exception>
        Task<RestResource<TOutput>> PostAsync<TInput, TOutput>(Uri url, RestResource<TInput> resource, RestResourceType outputType);

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type using the PUT HTTP method.
        /// </summary>
        /// <typeparam name="TInput">The input resource object type.</typeparam>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="resource">The input resource.</param>
        /// <returns>A task resulting in an output resource.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        /// <exception cref="ArgumentException">If the resource body is null.</exception>
        Task<RestResource<TOutput>> PutAsync<TInput, TOutput>(Uri url, RestResource<TInput> resource);

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type using the PUT HTTP method.
        /// </summary>
        /// <typeparam name="TInput">The input resource object type.</typeparam>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="resource">The input resource.</param>
        /// <param name="outputType">The output resource type.</param>
        /// <returns>A task resulting in an output resource.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        /// <exception cref="ArgumentException">If the resource body is null.</exception>
        Task<RestResource<TOutput>> PutAsync<TInput, TOutput>(Uri url, RestResource<TInput> resource, RestResourceType outputType);

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type using the PATCH HTTP method.
        /// </summary>
        /// <typeparam name="TInput">The input resource object type.</typeparam>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="resource">The input resource.</param>
        /// <returns>A task resulting in an output resource.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        /// <exception cref="ArgumentException">If the resource body is null.</exception>
        Task<RestResource<TOutput>> PatchAsync<TInput, TOutput>(Uri url, RestResource<TInput> resource);

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type using the PATCH HTTP method.
        /// </summary>
        /// <typeparam name="TInput">The input resource object type.</typeparam>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="resource">The input resource.</param>
        /// <param name="outputType">The output resource type.</param>
        /// <returns>A task resulting in an output resource.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        /// <exception cref="ArgumentException">If the resource body is null.</exception>
        Task<RestResource<TOutput>> PatchAsync<TInput, TOutput>(Uri url, RestResource<TInput> resource, RestResourceType outputType);

        /// <summary>
        /// Executes an HTTP request to the provided URL using the DELETE HTTP method.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>A task resulting in a collection of HTTP headers returned.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        Task<WebHeaderCollection> DeleteAsync(Uri url);

        /// <summary>
        /// Executes an HTTP request to the provided URL using the DELETE HTTP method.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="headers">A collection of HTTP headers to pass to the request.</param>
        /// <returns>A task resulting in a collection of HTTP headers returned.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        Task<WebHeaderCollection> DeleteAsync(Uri url, NameValueCollection headers);

        /// <summary>
        /// Executes an HTTP request to the provided URL using the OPTIONS HTTP method.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>A task resulting in a list of allowed HTTP methods.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        Task<IReadOnlyList<HttpMethod>> OptionsAsync(Uri url);
    }
}
