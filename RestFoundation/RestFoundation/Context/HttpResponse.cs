﻿// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using RestFoundation.Collections;
using RestFoundation.Collections.Concrete;
using RestFoundation.Resources;
using RestFoundation.Runtime;

namespace RestFoundation.Context
{
    /// <summary>
    /// Represents an HTTP response.
    /// </summary>
    public class HttpResponse : RestContextBase, IHttpResponse
    {
        private readonly IHttpResponseOutput m_output;
        private readonly ResponseHeaderNames m_headerNames;

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
            m_headerNames = new ResponseHeaderNames();
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
        /// Gets a value indicating whether the client is connected to the server.
        /// </summary>
        public bool IsClientConnected
        {
            get
            {
                return Context.Response.IsClientConnected;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the response status code is successful.
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                return Context.Response.StatusCode >= 200 && Context.Response.StatusCode < 300;
            }
        }

        /// <summary>
        /// Gets or sets a cancellation token source for a returned asynchronous task.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; set; }

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
        public ResponseHeaderNames HeaderNames
        {
            get
            {
                return m_headerNames;
            }
        }

        /// <summary>
        /// Appends or sets a response header value.
        /// </summary>
        /// <remarks>
        /// Some HTTP headers do not support multiple values. In that case the latest assigned
        /// value will be used.
        /// </remarks>
        /// <param name="headerName">The header name.</param>
        /// <param name="headerValue">The header value.</param>
        public void AppendHeader(string headerName, string headerValue)
        {
            SetHeader(headerName, headerValue, false);
        }

        /// <summary>
        /// Gets a collection of all response headers set by the service.
        /// </summary>
        /// <returns>A list of response headers.</returns>
        public IHeaderCollection GetHeaders()
        {
            return new HeaderCollection(Context.Response.Headers);
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
        /// <remarks>
        /// This behavior is not supported when running services under the Visual Studio
        /// Development Server. In that scenario the method has the same behavior as
        /// the <see cref="AppendHeader"/>.
        /// </remarks>
        /// <param name="headerName">The header name.</param>
        /// <param name="headerValue">The header value.</param>
        public void SetHeader(string headerName, string headerValue)
        {
            SetHeader(headerName, headerValue, true);
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
        /// Gets the response cancellation token.
        /// </summary>
        /// <returns>The response <see cref="CancellationToken"/>.</returns>
        public CancellationToken GetCancellationToken()
        {
            return CancellationTokenSource != null ? CancellationTokenSource.Token : CancellationToken.None;
        }

        /// <summary>
        /// Gets the response character encoding.
        /// </summary>
        /// <returns>Returns the encoding.</returns>
        public Encoding GetCharsetEncoding()
        {
            return Context.Response.ContentEncoding;
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
            SetStatus(statusCode, PascalCaseToSentenceConverter.Convert(statusCode.ToString()));
        }

        /// <summary>
        /// Sets the HTTP status code and description.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="statusDescription">The status description.</param>
        public void SetStatus(HttpStatusCode statusCode, string statusDescription)
        {
            Context.Response.StatusCode = (int) statusCode;
            Context.Response.StatusDescription = StatusDescriptionFormatter.Format(statusDescription);
        }

        /// <summary>
        /// Sets the HTTP status code and description.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="statusDescription">The status description.</param>
        public void SetStatus(int statusCode, string statusDescription)
        {
            if (statusCode < 100 || statusCode >= 600)
            {
                throw new ArgumentOutOfRangeException("statusCode");
            }

            Context.Response.StatusCode = statusCode;
            Context.Response.StatusDescription = StatusDescriptionFormatter.Format(statusDescription);
        }

        /// <summary>
        /// Gets a collection of all response cookies set by the service.
        /// </summary>
        /// <returns>A list of response cookies.</returns>
        public ICookieValueCollection GetCookies()
        {
            return new CookieValueCollection(Context.Response.Cookies);
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
        /// Sets a link header with the provided href and rel.
        /// </summary>
        /// <param name="href">The href URL.</param>
        /// <param name="rel">The relationship value.</param>
        /// <exception cref="ArgumentException">If href or rel contain invalid characters.</exception>
        [SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads",
                         Justification = "There is no reason to duplicate the Uri building logic.")]
        public void SetLink(string href, string rel)
        {
            SetLink(href, rel, null);
        }

        /// <summary>
        /// Sets a link header with the provided href, rel and title.
        /// </summary>
        /// <param name="href">The href URL.</param>
        /// <param name="rel">The relationship value.</param>
        /// <param name="title">The link title.</param>
        /// <exception cref="ArgumentException">If href or rel contain invalid characters.</exception>
        public void SetLink(string href, string rel, string title)
        {
            if (String.IsNullOrEmpty(href))
            {
                throw new ArgumentNullException("href");
            }

            Uri hrefUrl;

            if (!Uri.TryCreate(href, UriKind.RelativeOrAbsolute, out hrefUrl))
            {
                throw new ArgumentException(Resources.Global.InvalidFilePathOrUrl, "href");
            }

            SetLink(hrefUrl, rel, title);
        }

        /// <summary>
        /// Sets a link header with the provided href and rel.
        /// </summary>
        /// <param name="href">The href URL.</param>
        /// <param name="rel">The relationship value.</param>
        /// <exception cref="ArgumentException">If href or rel contain invalid characters.</exception>
        public void SetLink(Uri href, string rel)
        {
            SetLink(href, rel, null);
        }

        /// <summary>
        /// Sets a link header with the provided href, rel and title.
        /// </summary>
        /// <param name="href">The href URL.</param>
        /// <param name="rel">The relationship value.</param>
        /// <param name="title">The link title.</param>
        /// <exception cref="ArgumentException">If href or rel contain invalid characters.</exception>
        public void SetLink(Uri href, string rel, string title)
        {
            if (href == null)
            {
                throw new ArgumentNullException("href");
            }

            if (String.IsNullOrEmpty(rel))
            {
                throw new ArgumentNullException("rel");
            }

            string hrefString = href.ToString();

            if (hrefString.IndexOf('<') >= 0 || hrefString.IndexOf('>') >= 0)
            {
                throw new ArgumentException(Resources.Global.InvalidArgumentValue, "href");
            }

            if (rel.IndexOf('"') >= 0)
            {
                throw new ArgumentException(Resources.Global.InvalidArgumentValue, "rel");
            }

            var link = new StringBuilder();
            link.AppendFormat(CultureInfo.InvariantCulture, "<{0}>; rel=\"{1}\"", hrefString, rel);

            if (!String.IsNullOrWhiteSpace(title))
            {
                link.AppendFormat(CultureInfo.InvariantCulture, "; title=\"{0}\"", title.Replace('"', '\''));
            }

            SetHeader("Link", link.ToString());
        }

        /// <summary>
        /// Generates an e-tag for the file with the provided full path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The e-tag for the file.</returns>
        /// <exception cref="FileNotFoundException">
        /// If the provided file path is invalid.
        /// </exception>
        public string GenerateEtag(string filePath)
        {
            var file = new FileInfo(filePath);

            return GenerateEtag(file);
        }

        /// <summary>
        /// Generates an e-tag for the provided file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>The e-tag for the file.</returns>
        /// <exception cref="FileNotFoundException">
        /// If the provided file path is invalid.
        /// </exception>
        public string GenerateEtag(FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            if (!file.Exists)
            {
                throw new FileNotFoundException(Global.InvalidFilePathOrUrl, file.FullName);
            }

            string fileString = String.Concat(file.Name, ':', file.LastWriteTimeUtc.Ticks);

            using (var hashAlgorithm = MD5.Create())
            {
                var hash = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(fileString));

                return String.Format(CultureInfo.InvariantCulture, "\"{0}\"", Convert.ToBase64String(hash));
            }
        }

        /// <summary>
        /// Generates the last modified date for the file with the provided
        /// full path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>
        /// The last modified date of the file int the RFC-1123 format.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// If the provided file path is invalid.
        /// </exception>
        public string GenerateLastModifiedDate(string filePath)
        {
            var file = new FileInfo(filePath);

            return GenerateLastModifiedDate(file);
        }

        /// <summary>
        /// Generates the last modified date for the provided file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// The last modified date of the file int the RFC-1123 format.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// If the provided file path is invalid.
        /// </exception>
        public string GenerateLastModifiedDate(FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            if (!file.Exists)
            {
                throw new FileNotFoundException(Global.InvalidFilePathOrUrl, file.FullName);
            }

            return file.LastWriteTimeUtc.ToString("r", CultureInfo.InvariantCulture);
        }

        private void SetHeader(string headerName, string headerValue, bool overwriteHeader)
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
                throw new HttpResponseException(HttpStatusCode.InternalServerError, Resources.Global.EmptyHttpHeader);
            }

            if (overwriteHeader && HttpRuntime.UsingIntegratedPipeline)
            {
                Context.Response.Headers.Remove(headerName);
            }

            Context.Response.AppendHeader(headerName, headerValue);
        }
    }
}
