﻿// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Threading.Tasks;
using System.Web;
using RestFoundation.Runtime;

namespace RestFoundation.Client
{
    internal sealed class RestClient : IRestClient
    {
        private const int MinErrorStatusCode = 400;
        private const string ContentEncodingHeader = "Content-Encoding";

        private readonly IRestClientSerializerFactory m_serializerFactory;
        private readonly IDictionary<RestResourceType, string> m_resourceTypes;

        internal RestClient(IRestClientSerializerFactory serializerFactory, IDictionary<RestResourceType, string> resourceTypes)
        {
            if (serializerFactory == null)
            {
                throw new ArgumentNullException("serializerFactory");
            }

            if (resourceTypes == null)
            {
                throw new ArgumentNullException("resourceTypes");
            }

            m_serializerFactory = serializerFactory;
            m_resourceTypes = resourceTypes;
        }

        public bool PerformRedirects { get; set; }
        public bool AllowCookies { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public NetworkCredential Credentials { get; set; }
        public string AuthenticationType { get; set; }
        public string ProxyUrl { get; set; }
        public bool SupportsEncoding { get; set; }
        public bool AllowSelfSignedCertificates { get; set; }

        #region Public Methods

        public Task<RestResource> GetAsync(Uri url)
        {
            return GetAsync(url, null);
        }

        public Task<RestResource> GetAsync(Uri url, WebHeaderCollection headers)
        {
            return Execute(url, HttpMethod.Get, headers);
        }

        public Task<RestResource<TOutput>> GetAsync<TOutput>(Uri url, RestResourceType outputType)
        {
            return GetAsync<TOutput>(url, outputType, null, null);
        }

        public Task<RestResource<TOutput>> GetAsync<TOutput>(Uri url, RestResourceType outputType, WebHeaderCollection headers)
        {
            return GetAsync<TOutput>(url, outputType, headers, null);
        }

        public Task<RestResource<TOutput>> GetAsync<TOutput>(Uri url, RestResourceType outputType, WebHeaderCollection headers, string xmlNamespace)
        {
            return Execute<TOutput>(url, HttpMethod.Get, outputType, headers, xmlNamespace);
        }

        public Task<RestResource> HeadAsync(Uri url)
        {
            return HeadAsync(url, null);
        }

        public Task<RestResource> HeadAsync(Uri url, WebHeaderCollection headers)
        {
            return Execute(url, HttpMethod.Head, headers);
        }

        public Task<RestResource> HeadAsync(Uri url, RestResourceType outputType)
        {
            return HeadAsync(url, outputType, null, null);
        }

        public Task<RestResource> HeadAsync(Uri url, RestResourceType outputType, WebHeaderCollection headers)
        {
            return HeadAsync(url, outputType, headers, null);
        }

        public async Task<RestResource> HeadAsync(Uri url, RestResourceType outputType, WebHeaderCollection headers, string xmlNamespace)
        {
            RestResource<dynamic> resource = await Execute<dynamic>(url, HttpMethod.Head, outputType, headers, xmlNamespace);

            return GetResourceHeaders(resource);
        }

        public Task<RestResource<TOutput>> PostAsync<TInput, TOutput>(Uri url, RestResource<TInput> resource)
        {
            return PostAsync<TInput, TOutput>(url, resource, GetDefaultOutputResourceType(resource));
        }

        public Task<RestResource<TOutput>> PostAsync<TInput, TOutput>(Uri url, RestResource<TInput> resource, RestResourceType outputType)
        {
            return Execute<TInput, TOutput>(url, HttpMethod.Post, resource, outputType);
        }

        public Task<RestResource<TOutput>> PutAsync<TInput, TOutput>(Uri url, RestResource<TInput> resource)
        {
            return PutAsync<TInput, TOutput>(url, resource, GetDefaultOutputResourceType(resource));
        }

        public Task<RestResource<TOutput>> PutAsync<TInput, TOutput>(Uri url, RestResource<TInput> resource, RestResourceType outputType)
        {
            return Execute<TInput, TOutput>(url, HttpMethod.Put, resource, outputType);
        }

        public Task<RestResource<TOutput>> PatchAsync<TInput, TOutput>(Uri url, RestResource<TInput> resource)
        {
            return PatchAsync<TInput, TOutput>(url, resource, GetDefaultOutputResourceType(resource));
        }

        public Task<RestResource<TOutput>> PatchAsync<TInput, TOutput>(Uri url, RestResource<TInput> resource, RestResourceType outputType)
        {
            return Execute<TInput, TOutput>(url, HttpMethod.Patch, resource, outputType);
        }

        public Task<RestResource> DeleteAsync(Uri url)
        {
            return DeleteAsync(url, null);
        }

        public Task<RestResource> DeleteAsync(Uri url, WebHeaderCollection headers)
        {
            return Execute(url, HttpMethod.Delete, headers);
        }

        public async Task<IReadOnlyList<HttpMethod>> OptionsAsync(Uri url)
        {
            RestResource resource = await Execute(url, HttpMethod.Options, null);

            return AllowHeaderParser.Parse(resource.Headers);
        }

        #endregion

        #region Private Helper Methods
        
        private static RestResourceType GetDefaultOutputResourceType<TInput>(RestResource<TInput> resource)
        {
            return resource != null ? resource.Type : default(RestResourceType);
        }

        private static RestResource GetResourceHeaders(RestResource<dynamic> resource)
        {
            var responseHeaders = new WebHeaderCollection();

            if (resource != null && resource.Headers != null)
            {
                foreach (string headerName in resource.Headers.AllKeys)
                {
                    responseHeaders.Add(headerName, resource.Headers.Get(headerName));
                }
            }

            return new RestResource(RestResourceType.None, responseHeaders);
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

        private static void MergeHeaders(WebHeaderCollection headers, RestResource emptyResource)
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

        private static async Task<RestResource> CreateEmptyResponse(WebRequest request)
        {
            try
            {
                var responseTask = Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, request).ConfigureAwait(false);

                using (var response = (HttpWebResponse) await responseTask)
                {
                    if ((int) response.StatusCode >= MinErrorStatusCode)
                    {
                        throw new HttpException((int) response.StatusCode, response.StatusDescription);
                    }

                    return new RestResource(RestResourceType.None, response.Headers)
                    {
                        StatusCode = response.StatusCode,
                        StatusDescription = response.StatusDescription
                    };
                }
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.ProtocolError)
                {
                    throw new HttpException((int) HttpStatusCode.InternalServerError, ex.Message, ex);
                }

                throw GenerateHttpException(ex);
            }
        }

        private static HttpException GenerateHttpException(WebException ex)
        {
            var response = (HttpWebResponse) ex.Response;
            var httpException = new HttpException((int) response.StatusCode, response.StatusDescription, ex);

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

        private string GetMimeType(RestResourceType type)
        {
            string mimeType;

            if (!m_resourceTypes.TryGetValue(type, out mimeType) || String.IsNullOrWhiteSpace(mimeType))
            {
                throw new InvalidOperationException(Resources.Global.UnmappedResourceType);
            }

            return mimeType;
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

        private async Task SerializeInputResourceBody<T>(WebRequest request, RestResource resource)
        {
            if (resource.ResourceBody == null)
            {
                request.ContentLength = 0;
                return;
            }

            IRestClientSerializer serializer = m_serializerFactory.Create(typeof(T), resource.Type, resource.XmlNamespace);
            await serializer.SerializeAsync(request, resource.ResourceBody);
        }

        private async Task DeserializeResourceBody<T>(RestResource<T> outputResource, IRestClientSerializer serializer, HttpWebResponse response, Stream responseStream)
        {
            if (String.Equals("HEAD", response.Method, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (SupportsEncoding && String.Equals("gzip", response.Headers[ContentEncodingHeader], StringComparison.OrdinalIgnoreCase))
            {
                using (var uncompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
                {
                    outputResource.Body = await serializer.DeserializeAsync<T>(uncompressedStream);
                }
            }
            else if (SupportsEncoding && String.Equals("deflate", response.Headers[ContentEncodingHeader], StringComparison.OrdinalIgnoreCase))
            {
                using (var uncompressedStream = new DeflateStream(responseStream, CompressionMode.Decompress))
                {
                    outputResource.Body = await serializer.DeserializeAsync<T>(uncompressedStream);
                }
            }
            else
            {
                outputResource.Body = await serializer.DeserializeAsync<T>(responseStream);
            }
        }

        private HttpWebRequest CreateRequest(Uri url, HttpMethod method, RestResource resource)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.KeepAlive = true;
            request.Method = method.ToString().ToUpperInvariant();
            request.AllowAutoRedirect = PerformRedirects;
            request.Headers.Add(resource.Headers);

            if (UseDefaultCredentials)
            {
                request.UseDefaultCredentials = true;
            }
            else if (Credentials != null)
            {
                SetCredentials(request, url);
            }

            if (!String.IsNullOrWhiteSpace(ProxyUrl))
            {
                request.Proxy = new WebProxy(ProxyUrl);
            }

            return request;
        }

        private async Task<RestResource<T>> CreateResponse<T>(WebRequest request, RestResourceType outputType, string xmlNamespace)
        {
            RemoteCertificateValidationCallback validationCallback = null;

            try
            {
                if (AllowSelfSignedCertificates && request.RequestUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                {
                    validationCallback = ServicePointManager.ServerCertificateValidationCallback;
                    ServicePointManager.ServerCertificateValidationCallback = (obj, certificate, chain, errors) => true;
                }

                var responseTask = Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, request).ConfigureAwait(false);

                using (var response = (HttpWebResponse) await responseTask)
                {
                    if ((int) response.StatusCode >= MinErrorStatusCode)
                    {
                        throw new HttpException((int) response.StatusCode, response.StatusDescription);
                    }

                    var outputResource = new RestResource<T>(outputType, response.Headers)
                    {
                        StatusCode = response.StatusCode,
                        StatusDescription = response.StatusDescription
                    };

                    Stream responseStream = response.GetResponseStream();

                    IRestClientSerializer serializer = m_serializerFactory.Create(typeof(T), outputType, xmlNamespace);
                    await DeserializeResourceBody(outputResource, serializer, response, responseStream);

                    return outputResource;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.ProtocolError)
                {
                    throw new HttpException((int) HttpStatusCode.InternalServerError, ex.Message, ex);
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

        #endregion

        #region Private Execution Methods

        private Task<RestResource> Execute(Uri url, HttpMethod method, WebHeaderCollection headers)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            var emptyResource = new RestResource();

            if (headers != null && headers.Count > 0)
            {
                MergeHeaders(headers, emptyResource);
            }

            HttpWebRequest request = CreateRequest(url, method, emptyResource);

            if (headers != null && headers.AllKeys.Contains("accept", StringComparer.OrdinalIgnoreCase))
            {
                request.Accept = GetMimeType(emptyResource.Type);
            }

            return CreateEmptyResponse(request);
        }

        private Task<RestResource<TOutput>> Execute<TOutput>(Uri url, HttpMethod method, RestResourceType outputType, WebHeaderCollection headers, string xmlNamespace)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            var emptyResource = new RestResource();

            if (headers != null && headers.Count > 0)
            {
                MergeHeaders(headers, emptyResource);
            }

            var request = CreateRequest(url, method, emptyResource);
            request.Accept = GetMimeType(outputType);

            return CreateResponse<TOutput>(request, outputType, xmlNamespace);
        }

        private async Task<RestResource<TOutput>> Execute<TInput, TOutput>(Uri url, HttpMethod method, RestResource<TInput> resource, RestResourceType outputType)
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
                throw new ArgumentException(Resources.Global.NullResourceBody, "resource");
            }

            HttpWebRequest request = CreateRequest(url, method, resource);
            request.ContentType = GetMimeType(resource.Type);
            request.Accept = GetMimeType(outputType);

            if (AllowCookies)
            {
                request.CookieContainer = new CookieContainer();
            }

            await SerializeInputResourceBody<TInput>(request, resource);

            return await CreateResponse<TOutput>(request, outputType, resource.XmlNamespace);
        }

        #endregion
    }
}
