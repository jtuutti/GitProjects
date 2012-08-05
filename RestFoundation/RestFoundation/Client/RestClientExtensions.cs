// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Specialized;
using System.Net;
using System.Security;
using System.Web;

namespace RestFoundation.Client
{
    /// <summary>
    /// Provides extension methods to the <see cref="IRestClient"/> interface.
    /// </summary>
    public static class RestClientExtensions
    {
        /// <summary>
        /// Executes an HTTP request to the provided URL using the GET HTTP method.
        /// </summary>
        /// <param name="client">The HTTP client instance.</param>
        /// <param name="url">The URL.</param>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        public static void Get(this IRestClient client, Uri url)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            client.Execute(url, HttpMethod.Get);
        }

        /// <summary>
        /// Executes an HTTP request to the provided URL using the GET HTTP method.
        /// </summary>
        /// <param name="client">The HTTP client instance.</param>
        /// <param name="url">The URL.</param>
        /// <param name="headers">A collection of HTTP headers to pass to the request.</param>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        public static void Get(this IRestClient client, Uri url, NameValueCollection headers)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            client.Execute(url, HttpMethod.Get, headers);
        }

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type using the GET HTTP method.
        /// </summary>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="client">The HTTP client instance.</param>
        /// <param name="url">The URL.</param>
        /// <param name="outputType">The output resource type.</param>
        /// <returns>The output resource.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        public static RestResource<TOutput> Get<TOutput>(this IRestClient client, Uri url, RestResourceType outputType)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            return client.Execute<TOutput>(url, HttpMethod.Get, outputType);
        }

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type using the GET HTTP method.
        /// </summary>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="client">The HTTP client instance.</param>
        /// <param name="url">The URL.</param>
        /// <param name="outputType">The output resource type.</param>
        /// <param name="headers">A collection of HTTP headers to pass to the request.</param>
        /// <returns>The output resource.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        public static RestResource<TOutput> Get<TOutput>(this IRestClient client, Uri url, RestResourceType outputType, NameValueCollection headers)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            return client.Execute<TOutput>(url, HttpMethod.Get, outputType, headers);
        }

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type using the POST HTTP method.
        /// </summary>
        /// <typeparam name="TInput">The input resource object type.</typeparam>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="client">The HTTP client instance.</param>
        /// <param name="url">The URL.</param>
        /// <param name="resource">The input resource.</param>
        /// <returns>The output resource.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        /// <exception cref="ArgumentException">If the resource body is null.</exception>
        public static RestResource<TOutput> Post<TInput, TOutput>(this IRestClient client, Uri url, RestResource<TInput> resource)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            return client.Execute<TInput, TOutput>(url, HttpMethod.Post, resource);
        }

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type using the POST HTTP method.
        /// </summary>
        /// <typeparam name="TInput">The input resource object type.</typeparam>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="client">The HTTP client instance.</param>
        /// <param name="url">The URL.</param>
        /// <param name="resource">The input resource.</param>
        /// <param name="outputType">The output resource type.</param>
        /// <returns>The output resource.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        /// <exception cref="ArgumentException">If the resource body is null.</exception>
        public static RestResource<TOutput> Post<TInput, TOutput>(this IRestClient client, Uri url, RestResource<TInput> resource, RestResourceType outputType)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            return client.Execute<TInput, TOutput>(url, HttpMethod.Post, resource, outputType);
        }

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type using the PUT HTTP method.
        /// </summary>
        /// <typeparam name="TInput">The input resource object type.</typeparam>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="client">The HTTP client instance.</param>
        /// <param name="url">The URL.</param>
        /// <param name="resource">The input resource.</param>
        /// <returns>The output resource.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        /// <exception cref="ArgumentException">If the resource body is null.</exception>
        public static RestResource<TOutput> Put<TInput, TOutput>(this IRestClient client, Uri url, RestResource<TInput> resource)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            return client.Execute<TInput, TOutput>(url, HttpMethod.Put, resource);
        }

        /// <summary>
        /// Executes an HTTP request to the provided URL and outputs an expected resource of the specified type using the PUT HTTP method.
        /// </summary>
        /// <typeparam name="TInput">The input resource object type.</typeparam>
        /// <typeparam name="TOutput">The output resource object type.</typeparam>
        /// <param name="client">The HTTP client instance.</param>
        /// <param name="url">The URL.</param>
        /// <param name="resource">The input resource.</param>
        /// <param name="outputType">The output resource type.</param>
        /// <returns>The output resource.</returns>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        /// <exception cref="InvalidOperationException">If a resource object is not serializable.</exception>
        /// <exception cref="ArgumentException">If the resource body is null.</exception>
        public static RestResource<TOutput> Put<TInput, TOutput>(this IRestClient client, Uri url, RestResource<TInput> resource, RestResourceType outputType)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            return client.Execute<TInput, TOutput>(url, HttpMethod.Put, resource, outputType);
        }

        /// <summary>
        /// Executes an HTTP request to the provided URL using the DELETE HTTP method.
        /// </summary>
        /// <param name="client">The HTTP client instance.</param>
        /// <param name="url">The URL.</param>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        public static void Delete(this IRestClient client, Uri url)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            client.Execute(url, HttpMethod.Delete);
        }

        /// <summary>
        /// Executes an HTTP request to the provided URL using the DELETE HTTP method.
        /// </summary>
        /// <param name="client">The HTTP client instance.</param>
        /// <param name="url">The URL.</param>
        /// <param name="headers">A collection of HTTP headers to pass to the request.</param>
        /// <exception cref="HttpException">If an HTTP-level exception occurred.</exception>
        /// <exception cref="WebException">If a non-HTTP level exception occurred.</exception>
        /// <exception cref="ProtocolViolationException">If an unexpected protocol exception occurred.</exception>
        /// <exception cref="SecurityException">If a security exception occurred.</exception>
        public static void Delete(this IRestClient client, Uri url, NameValueCollection headers)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            client.Execute(url, HttpMethod.Delete, headers);
        }
    }
}
