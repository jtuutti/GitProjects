// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Threading.Tasks;
using System.Web;

namespace RestFoundation.Client
{
    internal sealed class RestClient : IRestClient
    {
        private const int MinErrorCode = 400;
        private const string InvalidAsyncResult = "Invalid asynchronous result was provided.";
        private const string ContentEncodingHeader = "Content-Encoding";

        private readonly IRestSerializerFactory m_factory;
        private readonly IDictionary<RestResourceType, string> m_httpResourceMap;

        internal RestClient(IRestSerializerFactory factory, IDictionary<RestResourceType, string> resourceMap)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            if (resourceMap == null)
            {
                throw new ArgumentNullException("resourceMap");
            }

            m_factory = factory;
            m_httpResourceMap = resourceMap;
        }

        public TimeSpan ConnectionTimeout { get; set; }
        public TimeSpan SocketTimeout { get; set; }
        public bool PerformRedirects { get; set; }
        public bool AllowCookies { get; set; }
        public NetworkCredential Credentials { get; set; }
        public string AuthenticationType { get; set; }
        public string ProxyUrl { get; set; }
        public bool SupportsEncoding { get; set; }
        public bool AllowSelfSignedCertificates { get; set; }

        public int LastStatusCode { get; private set; }
        public string LastStatusDescription { get; private set; }

        public void Execute(Uri url, HttpMethod method)
        {
            Execute(url, method, null);
        }

        public void Execute(Uri url, HttpMethod method, NameValueCollection headers)
        {
            var emptyResource = new RestResource();

            if (headers != null && headers.Count > 0)
            {
                MergeHeaders(headers, emptyResource);
            }

            var request = CreateRequest(url, method, emptyResource);
            ProcessEmptyResponse(request);
        }

        public void Execute<TInput>(Uri url, HttpMethod method, RestResource<TInput> resource)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            if (resource == null)
            {
                throw new ArgumentNullException("resource");
            }

            if (ReferenceEquals(resource.Body, null))
            {
                throw new ArgumentException("Resource body cannot be null", "resource");
            }

            var request = CreateRequest(url, method, resource);
            request.ContentType = GetMimeType(resource.Type);

            if (AllowCookies)
            {
                request.CookieContainer = new CookieContainer();
            }

            ProcessBody<TInput>(request, resource);
            ProcessEmptyResponse(request);
        }

        public RestResource<TOutput> Execute<TOutput>(Uri url, HttpMethod method, RestResourceType outputType)
        {
            return Execute<TOutput>(url, method, outputType, null);
        }

        public RestResource<TOutput> Execute<TOutput>(Uri url, HttpMethod method, RestResourceType outputType, NameValueCollection headers)
        {
            var emptyResource = new RestResource();

            if (headers != null && headers.Count > 0)
            {
                MergeHeaders(headers, emptyResource);
            }

            var request = CreateRequest(url, method, emptyResource);
            request.Accept = GetMimeType(outputType);

            return ProcessResponse<TOutput>(request, outputType);
        }

        public RestResource<TOutput> Execute<TInput, TOutput>(Uri url, HttpMethod method, RestResource<TInput> resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException("resource");
            }

            return Execute<TInput, TOutput>(url, method, resource, resource.Type);
        }

        public RestResource<TOutput> Execute<TInput, TOutput>(Uri url, HttpMethod method, RestResource<TInput> resource, RestResourceType outputType)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            if (resource == null)
            {
                throw new ArgumentNullException("resource");
            }

            if (ReferenceEquals(resource.Body, null))
            {
                throw new ArgumentException("Resource body cannot be null", "resource");
            }

            var request = CreateRequest(url, method, resource);
            request.ContentType = GetMimeType(resource.Type);
            request.Accept = GetMimeType(outputType);

            if (AllowCookies)
            {
                request.CookieContainer = new CookieContainer();
            }

            ProcessBody<TInput>(request, resource);

            return ProcessResponse<TOutput>(request, outputType);
        }

        public IAsyncResult BeginExecute<TOutput>(Uri url, HttpMethod method, RestResourceType outputType, NameValueCollection headers, AsyncCallback callback)
        {
            var task = Task<RestResource<TOutput>>.Factory.StartNew(() => Execute<TOutput>(url, method, outputType, headers));

            if (callback != null)
            {
                task.ContinueWith(asyncResult => callback(asyncResult));
            }

            return task;
        }

        public IAsyncResult BeginExecute<TInput, TOutput>(Uri url, HttpMethod method, RestResource<TInput> resource, RestResourceType outputType, AsyncCallback callback)
        {
            var task = Task<RestResource<TOutput>>.Factory.StartNew(() => Execute<TInput, TOutput>(url, method, resource, outputType));

            if (callback != null)
            {
                task.ContinueWith(asyncResult => callback(asyncResult));
            }

            return task;
        }

        public RestResource<TOutput> EndExecute<TOutput>(IAsyncResult result)
        {
            var task = result as Task<RestResource<TOutput>>;

            if (task == null)
            {
                throw new InvalidOperationException(InvalidAsyncResult);
            }

            if (task.IsFaulted && task.Exception != null)
            {
                throw task.Exception;
            }

            return task.Result;
        }

        private static void MergeHeaders(NameValueCollection headers, RestResource emptyResource)
        {
            foreach (string key in headers.AllKeys)
            {
                string value = headers.Get(key);

                if (value == null)
                {
                    continue;
                }

                if (String.Equals(key.Trim(), "accept", StringComparison.OrdinalIgnoreCase))
                {
                    if (!headers.AllKeys.Contains("content-type", StringComparer.OrdinalIgnoreCase))
                    {
                        SetResourceTypeFromHeader(emptyResource, value);
                    }

                    continue;
                }

                if (String.Equals(key.Trim(), "content-type", StringComparison.OrdinalIgnoreCase))
                {
                    SetResourceTypeFromHeader(emptyResource, value);
                    continue;
                }

                emptyResource.Headers.Add(key, value);
            }
        }

        private static void SetResourceTypeFromHeader(RestResource emptyResource, string value)
        {
            value = value.Trim().ToLowerInvariant();

            if (value.Contains("application/json"))
            {
                emptyResource.Type = RestResourceType.Json;
            }
            else if (value.Contains("application/xml") || value.Contains("text/xml"))
            {
                emptyResource.Type = RestResourceType.Xml;
            }
        }

        private void ProcessBody<T>(WebRequest request, RestResource resource)
        {
            if (resource.ResourceBody != null)
            {
                IRestSerializer serializer = m_factory.Create(typeof(T), resource.Type);
                request.ContentLength = serializer.GetContentLength(resource.ResourceBody);
                serializer.Serialize(request.GetRequestStream(), resource.ResourceBody);
            }
            else
            {
                request.ContentLength = 0;
            }
        }

        private string GetMimeType(RestResourceType type)
        {
            string mimeType;

            if (!m_httpResourceMap.TryGetValue(type, out mimeType) || String.IsNullOrWhiteSpace(mimeType))
            {
                throw new InvalidOperationException("HTTP resource type provided has not been mapped.");
            }

            return mimeType;
        }

        private HttpWebRequest CreateRequest(Uri url, HttpMethod method, RestResource resource)
        {
            if (ConnectionTimeout.TotalMilliseconds <= 0)
            {
                throw new TimeoutException("Connection timeout is invalid.");
            }

            if (SocketTimeout.TotalMilliseconds <= 0)
            {
                throw new TimeoutException("Socket timeout is invalid.");
            }

            var request = (HttpWebRequest) WebRequest.Create(url);
            request.KeepAlive = true;
            request.Method = method.ToString().ToUpperInvariant();
            request.AllowAutoRedirect = PerformRedirects;
            request.Timeout = Convert.ToInt32(ConnectionTimeout.TotalMilliseconds);
            request.ReadWriteTimeout = Convert.ToInt32(SocketTimeout.TotalMilliseconds);
            request.Headers.Add(resource.Headers);

            if (Credentials != null)
            {
                SetCredentials(request, url);
            }

            if (!String.IsNullOrWhiteSpace(ProxyUrl))
            {
                request.Proxy = new WebProxy(ProxyUrl);
            }

            return request;
        }

        private void SetCredentials(HttpWebRequest request, Uri url)
        {
            string authenticationType = !String.IsNullOrEmpty(AuthenticationType) ? AuthenticationType : "Basic";
            Uri domainUrl;

            var credentialCache = new CredentialCache();

            if (Uri.TryCreate(Credentials.Domain, UriKind.RelativeOrAbsolute, out domainUrl))
            {
                credentialCache.Add(domainUrl, authenticationType, Credentials);
            }
            else
            {
                credentialCache.Add(url, authenticationType, Credentials);
            }

            request.Credentials = credentialCache;
        }

        private RestResource<T> ProcessResponse<T>(WebRequest request, RestResourceType outputType)
        {
            RemoteCertificateValidationCallback validationCallback = null;

            try
            {
                if (AllowSelfSignedCertificates && request.RequestUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                {
                    validationCallback = ServicePointManager.ServerCertificateValidationCallback;
                    ServicePointManager.ServerCertificateValidationCallback = (obj, certificate, chain, errors) => true;
                }

                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    LastStatusCode = (int) response.StatusCode;
                    LastStatusDescription = response.StatusDescription;

                    if (LastStatusCode >= MinErrorCode)
                    {
                        throw new HttpException(LastStatusCode, response.StatusDescription);
                    }

                    var outputResource = new RestResource<T>(outputType, response.Headers);
                    Stream responseStream = response.GetResponseStream();

                    IRestSerializer serializer = m_factory.Create(typeof(T), outputType);

                    DeserializeResourceBody(outputResource, serializer, response, responseStream);

                    return outputResource;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.ProtocolError)
                {
                    LastStatusCode = (int) HttpStatusCode.InternalServerError;
                    LastStatusDescription = ex.Message;

                    throw new HttpException(LastStatusCode, LastStatusDescription, ex);
                }

                throw GenerateHttpException(ex);
            }
            finally
            {
                if (validationCallback != null)
                {
                    ServicePointManager.ServerCertificateValidationCallback = validationCallback;
                }
            }
        }

        private void DeserializeResourceBody<T>(RestResource<T> outputResource, IRestSerializer serializer, WebResponse response, Stream responseStream)
        {
            if (SupportsEncoding && String.Equals("gzip", response.Headers[ContentEncodingHeader], StringComparison.OrdinalIgnoreCase))
            {
                using (var uncompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
                {
                    outputResource.Body = serializer.Deserialize<T>(uncompressedStream);
                }
            }
            else if (SupportsEncoding && String.Equals("deflate", response.Headers[ContentEncodingHeader], StringComparison.OrdinalIgnoreCase))
            {
                using (var uncompressedStream = new DeflateStream(responseStream, CompressionMode.Decompress))
                {
                    outputResource.Body = serializer.Deserialize<T>(uncompressedStream);
                }
            }
            else
            {
                outputResource.Body = serializer.Deserialize<T>(responseStream);
            }
        }

        private void ProcessEmptyResponse(WebRequest request)
        {
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    LastStatusCode = (int)response.StatusCode;
                    LastStatusDescription = response.StatusDescription;

                    if (LastStatusCode >= MinErrorCode)
                    {
                        throw new HttpException(LastStatusCode, response.StatusDescription);
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.ProtocolError)
                {
                    LastStatusCode = (int)HttpStatusCode.InternalServerError;
                    LastStatusDescription = ex.Message;

                    throw new HttpException(LastStatusCode, LastStatusDescription, ex);
                }

                throw GenerateHttpException(ex);
            }
        }

        private HttpException GenerateHttpException(WebException ex)
        {
            var response = (HttpWebResponse) ex.Response;

            LastStatusCode = (int) response.StatusCode;
            LastStatusDescription = response.StatusDescription;

            var httpException = new HttpException(LastStatusCode, LastStatusDescription, ex);

            foreach (string header in response.Headers.AllKeys)
            {
                string[] headerValues = response.Headers.GetValues(header);

                if (headerValues != null && headerValues.Length > 0)
                {
                    httpException.Data[header] = String.Join(",", headerValues);
                }
            }

            return httpException;
        }
    }
}
