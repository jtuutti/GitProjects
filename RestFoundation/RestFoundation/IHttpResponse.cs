// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Net;
using System.Text;
using System.Web;
using RestFoundation.Collections;

namespace RestFoundation
{
    /// <summary>
    /// Defines an HTTP response.
    /// </summary>
    public interface IHttpResponse
    {
        /// <summary>
        /// Gets the response output.
        /// </summary>
        IHttpResponseOutput Output { get; }

        /// <summary>
        /// Gets a value indicating whether the response status code is successful.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// Gets or sets a value indicating whether custom IIS 7+ error pages should be
        /// skipped, if possible.
        /// </summary>
        bool TrySkipIisCustomErrors { get; set; }

        /// <summary>
        /// Gets response header names.
        /// </summary>
        ResponseHeaderNames HeaderNames { get; }

        /// <summary>
        /// Gets a collection of all response headers set by the service.
        /// </summary>
        /// <returns>A list of response headers.</returns>
        IHeaderCollection GetHeaders();

        /// <summary>
        /// Gets a response header value by name.
        /// </summary>
        /// <param name="headerName">The header name.</param>
        /// <returns>The header value.</returns>
        string GetHeader(string headerName);

        /// <summary>
        /// Sets a response header.
        /// </summary>
        /// <param name="headerName">The header name.</param>
        /// <param name="headerValue">The header value.</param>
        void SetHeader(string headerName, string headerValue);

        /// <summary>
        /// Removes a response header by name.
        /// </summary>
        /// <param name="headerName">The header name.</param>
        /// <returns>
        /// true if the header was removed successfully, false if the header
        /// had not been a part of the response headers.
        /// </returns>
        bool RemoveHeader(string headerName);

        /// <summary>
        /// Clears all response headers.
        /// </summary>
        void ClearHeaders();

        /// <summary>
        /// Gets the response character encoding.
        /// </summary>
        /// <returns>Returns the encoding.</returns>
        Encoding GetCharsetEncoding();

        /// <summary>
        /// Sets the response character encoding.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        void SetCharsetEncoding(Encoding encoding);

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        /// <returns>The status code.</returns>
        HttpStatusCode GetStatusCode();

        /// <summary>
        /// Gets the HTTP status description.
        /// </summary>
        /// <returns>The status description.</returns>
        string GetStatusDescription();

        /// <summary>
        /// Sets the HTTP status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        void SetStatus(HttpStatusCode statusCode);

        /// <summary>
        /// Sets the HTTP status code and description.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="statusDescription">The status description.</param>
        void SetStatus(HttpStatusCode statusCode, string statusDescription);

        /// <summary>
        /// Gets a collection of all response cookies set by the service.
        /// </summary>
        /// <returns>A list of response cookies.</returns>
        ICookieValueCollection GetCookies();

        /// <summary>
        /// Gets a response cookie by name.
        /// </summary>
        /// <param name="cookieName">The cookie name.</param>
        /// <returns>The cookie object.</returns>
        HttpCookie GetCookie(string cookieName);

        /// <summary>
        /// Sets a response cookie.
        /// </summary>
        /// <param name="cookie">The cookie object.</param>
        void SetCookie(HttpCookie cookie);

        /// <summary>
        /// Expires a response cookie.
        /// </summary>
        /// <param name="cookie">The cookie object.</param>
        void ExpireCookie(HttpCookie cookie);

        /// <summary>
        /// Sets a link header with the provided href and rel.
        /// </summary>
        /// <param name="href">The href URL.</param>
        /// <param name="rel">The relationship value.</param>
        void SetLink(string href, string rel);

        /// <summary>
        /// Sets a link header with the provided href, rel and title.
        /// </summary>
        /// <param name="href">The href URL.</param>
        /// <param name="rel">The relationship value.</param>
        /// <param name="title">The link title.</param>
        void SetLink(string href, string rel, string title);

        /// <summary>
        /// Sets a link header with the provided href and rel.
        /// </summary>
        /// <param name="href">The href URL.</param>
        /// <param name="rel">The relationship value.</param>
        void SetLink(Uri href, string rel);

        /// <summary>
        /// Sets a link header with the provided href, rel and title.
        /// </summary>
        /// <param name="href">The href URL.</param>
        /// <param name="rel">The relationship value.</param>
        /// <param name="title">The link title.</param>
        void SetLink(Uri href, string rel, string title);

        /// <summary>
        /// Adds the file as a response dependency to create an e-tag, last modified time and
        /// set appropriate caching parameters.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        void SetFileDependencies(string filePath);

        /// <summary>
        /// Adds the file as a response dependency to create an e-tag, last modified time and
        /// set appropriate caching parameters.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="maxAge">The time span before the cache expires.</param>
        void SetFileDependencies(string filePath, TimeSpan maxAge);

        /// <summary>
        /// Adds the file as a response dependency to create an e-tag, last modified time and
        /// set appropriate caching parameters.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="cacheability">The cacheability value.</param>
        /// <param name="maxAge">The time span before the cache expires.</param>
        void SetFileDependencies(string filePath, HttpCacheability cacheability, TimeSpan maxAge);
    }
}
