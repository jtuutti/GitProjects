// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Net;
using System.Text;
using System.Web;
using RestFoundation.Runtime;

namespace RestFoundation.Context
{
    /// <summary>
    /// Represents an HTTP response.
    /// </summary>
    public class HttpResponse : ContextBase, IHttpResponse
    {
        private readonly IHttpResponseOutput m_output;
        private readonly ResponseHeaderNames m_headers;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponse"/> class.
        /// </summary>
        /// <param name="output">The HTTP response output.</param>
        public HttpResponse(IHttpResponseOutput output)
        {
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            m_output = output;
            m_headers = new ResponseHeaderNames();
        }

        /// <summary>
        /// Gets the response output.
        /// </summary>
        public IHttpResponseOutput Output
        {
            get
            {
                return m_output;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether custom IIS 7+ error pages should be
        /// skipped, if possible.
        /// </summary>
        public bool TrySkipIisCustomErrors
        {
            get
            {
                return Context.Response.TrySkipIisCustomErrors;
            }
            set
            {
                Context.Response.TrySkipIisCustomErrors = value;
            }
        }

        /// <summary>
        /// Gets response header names.
        /// </summary>
        public ResponseHeaderNames Headers
        {
            get
            {
                return m_headers;
            }
        }

        /// <summary>
        /// Gets a response header value by name.
        /// </summary>
        /// <param name="headerName">The header name.</param>
        /// <returns>The header value.</returns>
        public string GetHeader(string headerName)
        {
            if (String.IsNullOrEmpty(headerName))
            {
                throw new ArgumentNullException("headerName");
            }

            return Context.Response.Headers.Get(headerName);
        }

        /// <summary>
        /// Sets a response header.
        /// </summary>
        /// <param name="headerName">The header name.</param>
        /// <param name="headerValue">The header value.</param>
        public void SetHeader(string headerName, string headerValue)
        {
            if (String.IsNullOrEmpty(headerName))
            {
                throw new ArgumentNullException("headerName");
            }

            if (headerValue == null)
            {
                throw new ArgumentNullException("headerValue");
            }

            if (!HeaderNameValidator.IsValid(headerName))
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, RestResources.EmptyHttpHeader);
            }

            Context.Response.AppendHeader(headerName, headerValue);
        }

        /// <summary>
        /// Removes a response header by name.
        /// </summary>
        /// <param name="headerName">The header name.</param>
        /// <returns>
        /// true if the header was removed successfully, false if the header
        /// had not been a part of the response headers.
        /// </returns>
        public bool RemoveHeader(string headerName)
        {
            if (headerName == null)
            {
                throw new ArgumentNullException("headerName");
            }

            if (Array.IndexOf(Context.Response.Headers.AllKeys, headerName) < 0)
            {
                return false;
            }

            Context.Response.Headers.Remove(headerName);
            return true;
        }

        /// <summary>
        /// Clears all response headers.
        /// </summary>
        public void ClearHeaders()
        {
            Context.Response.ClearHeaders();
        }

        /// <summary>
        /// Sets the response character encoding.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        public void SetCharsetEncoding(Encoding encoding)
        {
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }

            Context.Response.ContentEncoding = encoding;
        }

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        /// <returns>The status code.</returns>
        public HttpStatusCode GetStatusCode()
        {
            return (HttpStatusCode) Context.Response.StatusCode;
        }

        /// <summary>
        /// Gets the HTTP status description.
        /// </summary>
        /// <returns>The status description.</returns>
        public string GetStatusDescription()
        {
            return Context.Response.StatusDescription;
        }

        /// <summary>
        /// Sets the HTTP status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        public void SetStatus(HttpStatusCode statusCode)
        {
            SetStatus(statusCode, String.Empty);
        }

        /// <summary>
        /// Sets the HTTP status code and description.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="statusDescription">The status description.</param>
        public void SetStatus(HttpStatusCode statusCode, string statusDescription)
        {
            Context.Response.StatusCode = (int) statusCode;
            Context.Response.StatusDescription = FormatStatusDescription(statusDescription);
        }

        /// <summary>
        /// Gets a response cookie by name.
        /// </summary>
        /// <param name="cookieName">The cookie name.</param>
        /// <returns>The cookie object.</returns>
        public HttpCookie GetCookie(string cookieName)
        {
            if (String.IsNullOrEmpty(cookieName))
            {
                throw new ArgumentNullException("cookieName");
            }

            return Context.Response.Cookies.Get(cookieName);
        }

        /// <summary>
        /// Sets a response cookie.
        /// </summary>
        /// <param name="cookie">The cookie object.</param>
        public void SetCookie(HttpCookie cookie)
        {
            if (cookie == null)
            {
                throw new ArgumentNullException("cookie");
            }

            Context.Response.SetCookie(cookie);
        }

        /// <summary>
        /// Expires a response cookie.
        /// </summary>
        /// <param name="cookie">The cookie object.</param>
        public void ExpireCookie(HttpCookie cookie)
        {
            if (cookie == null)
            {
                throw new ArgumentNullException("cookie");
            }

            cookie.Expires = DateTime.Now.AddDays(-1);
            Context.Response.SetCookie(cookie);
        }

        /// <summary>
        /// Adds the file as a response dependency to create an e-tag, last modified time and
        /// set appropriate caching parameters.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void SetFileDependencies(string filePath)
        {
            SetFileDependencies(filePath, HttpCacheability.ServerAndPrivate, TimeSpan.Zero);
        }

        /// <summary>
        /// Adds the file as a response dependency to create an e-tag, last modified time and
        /// set appropriate caching parameters.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="maxAge">The time span before the cache expires.</param>
        public void SetFileDependencies(string filePath, TimeSpan maxAge)
        {
            SetFileDependencies(filePath, HttpCacheability.ServerAndPrivate, maxAge);
        }

        /// <summary>
        /// Adds the file as a response dependency to create an e-tag, last modified time and
        /// set appropriate caching parameters.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="cacheability">The cacheability value.</param>
        /// <param name="maxAge">The time span before the cache expires.</param>
        public void SetFileDependencies(string filePath, HttpCacheability cacheability, TimeSpan maxAge)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath");
            }

            Context.Response.AddFileDependency(filePath);
            Context.Response.Cache.SetCacheability(cacheability);
            Context.Response.Cache.SetETagFromFileDependencies();
            Context.Response.Cache.SetLastModifiedFromFileDependencies();

            if (maxAge != TimeSpan.Zero)
            {
                Context.Response.Cache.SetMaxAge(maxAge);
            }

            Context.Response.Cache.VaryByParams["*"] = true;

            foreach (string headerName in Context.Request.Headers.AllKeys)
            {
                if (headerName.StartsWith("Accept", StringComparison.OrdinalIgnoreCase) ||
                    headerName.StartsWith("X-", StringComparison.OrdinalIgnoreCase))
                {
                    Context.Response.Cache.VaryByHeaders[headerName] = true;
                }
            }
        }

        private static string FormatStatusDescription(string statusDescription)
        {
            if (statusDescription == null)
            {
                return String.Empty;
            }

            var lineBreakCharacters = new[] { '\r', '\n' };

            if (statusDescription.IndexOfAny(lineBreakCharacters) >= 0)
            {
                statusDescription = statusDescription.Split(lineBreakCharacters, StringSplitOptions.None)[0];
            }

            const int MaxStatusDescriptionLength = 512;

            if (statusDescription.Length > MaxStatusDescriptionLength)
            {
                statusDescription = statusDescription.Substring(0, MaxStatusDescriptionLength);
            }

            return statusDescription.Trim();
        }
    }
}
