// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RestFoundation.Collections
{
    /// <summary>
    /// Defines an HTTP header collection.
    /// </summary>
    public interface IHeaderCollection : IStringValueCollection
    {
        /// <summary>
        /// Gets the Accept-Type header value.
        /// </summary>
        string AcceptType { get; }

        /// <summary>
        /// Gets the Accept-Type header values as a sequence in the order of preference.
        /// </summary>
        IEnumerable<string> AcceptTypes { get; }

        /// <summary>
        /// Gets the Accept-Charset header value.
        /// </summary>
        string AcceptCharset { get; }

        /// <summary>
        /// Gets the Accept-Charset header values as a sequence in the order of preference.
        /// </summary>
        IEnumerable<string> AcceptCharsets { get; }

        /// <summary>
        /// Gets the Accept-Charset header value as an <see cref="Encoding"/> object.
        /// </summary>
        Encoding AcceptCharsetEncoding { get; }

        /// <summary>
        /// Gets the Accept-Encoding header value.
        /// </summary>
        string AcceptEncoding { get; }

        /// <summary>
        /// Gets the Accept-Encoding header values as a sequence in the order of preference.
        /// </summary>
        IEnumerable<string> AcceptEncodings { get; }

        /// <summary>
        /// Gets the Accept-Language header value.
        /// </summary>
        string AcceptLanguage { get; }

        /// <summary>
        /// Gets the Accept-Language header values as a sequence in the order of preference.
        /// </summary>
        IEnumerable<string> AcceptLanguages { get; }

        /// <summary>
        /// Gets the Accept-Language header value as a <see cref="CultureInfo"/> object.
        /// </summary>
        CultureInfo AcceptLanguageCulture { get; }
        
        /// <summary>
        /// Gets the version specified in the version parameter of the mime type provided in the
        /// Accept header value. This value defaults to 0 if no version was specified.
        /// </summary>
        decimal AcceptVersion { get; }

        /// <summary>
        /// Gets the Content-Type header value.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Gets the charset part of the Content-Type header value.
        /// </summary>
        string ContentCharset { get; }

        /// <summary>
        /// Gets the charset part of the Content-Type header value as an <see cref="Encoding"/> object.
        /// </summary>
        Encoding ContentCharsetEncoding { get; }

        /// <summary>
        /// Gets the Content-Encoding header value.
        /// </summary>
        string ContentEncoding { get; }

        /// <summary>
        /// Gets the Content-Length header value.
        /// </summary>
        int ContentLength { get; }

        /// <summary>
        /// Gets the Content-Language header value.
        /// </summary>
        string ContentLanguage { get; }

        /// <summary>
        /// Gets the Content-Language header value as a <see cref="CultureInfo"/> object.
        /// </summary>
        CultureInfo ContentLanguageCulture { get; }

        /// <summary>
        /// Gets the Authorization header value.
        /// </summary>
        string Authorization { get; }

        /// <summary>
        /// Gets the Host header value.
        /// </summary>
        string Host { get; }

        /// <summary>
        /// Gets the Referrer header value.
        /// </summary>
        string Referrer { get; }

        /// <summary>
        /// Gets the User-Agent header value.
        /// </summary>
        string UserAgent { get; }
    }
}
