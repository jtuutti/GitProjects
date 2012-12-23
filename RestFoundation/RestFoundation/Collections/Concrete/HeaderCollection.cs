// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using RestFoundation.Collections.Specialized;
using RestFoundation.Runtime;

namespace RestFoundation.Collections.Concrete
{
    /// <summary>
    /// Represents an HTTP header collection.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public class HeaderCollection : StringValueCollection, IHeaderCollection
    {
        private const string Utf8Charset = "utf-8";

        internal HeaderCollection(NameValueCollection values) : base(values)
        {
            Authorization = TryGet("Authorization");
            Host = TryGet("Host");
            Referrer = TryGet("Referrer");
            UserAgent = TryGet("User-Agent");

            ContentLength = GetContentLength();
            ContentEncoding = TryGet("Content-Encoding");

            var contentTypes = new AcceptValueCollection(TryGet("Content-Type"));
            ContentType = contentTypes.GetPreferredName();
            SetContentCharset(contentTypes);
            SetContentLanguage();

            SetAcceptValues();
            SetAcceptCharsets();
            SetAcceptEncodings();
            SetAcceptLanguages();
        }

        /// <summary>
        /// Gets the Accept-Type header value.
        /// </summary>
        public string AcceptType { get; protected set; }

        /// <summary>
        /// Gets the Accept-Type header values as a sequence in the order of preference.
        /// </summary>
        public IEnumerable<string> AcceptTypes { get; protected set; }

        /// <summary>
        /// Gets the Accept-Charset header value.
        /// </summary>
        public string AcceptCharset { get; protected set; }

        /// <summary>
        /// Gets the Accept-Charset header values as a sequence in the order of preference.
        /// </summary>
        public IEnumerable<string> AcceptCharsets { get; protected set; }

        /// <summary>
        /// Gets the Accept-Charset header value as an <see cref="Encoding"/> object.
        /// </summary>
        public Encoding AcceptCharsetEncoding { get; protected set; }

        /// <summary>
        /// Gets the Accept-Encoding header value.
        /// </summary>
        public string AcceptEncoding { get; protected set; }

        /// <summary>
        /// Gets the Accept-Encoding header values as a sequence in the order of preference.
        /// </summary>
        public IEnumerable<string> AcceptEncodings { get; protected set; }

        /// <summary>
        /// Gets the Accept-Language header value.
        /// </summary>
        public string AcceptLanguage { get; protected set; }

        /// <summary>
        /// Gets the Accept-Language header values as a sequence in the order of preference.
        /// </summary>
        public IEnumerable<string> AcceptLanguages { get; protected set; }

        /// <summary>
        /// Gets the Accept-Language header value as a <see cref="CultureInfo"/> object.
        /// </summary>
        public CultureInfo AcceptLanguageCulture { get; protected set; }

        /// <summary>
        /// Gets the version specified in the version parameter of the mime type provided in the
        /// Accept header value. This value defaults to 0 if no version was specified.
        /// </summary>
        public decimal AcceptVersion { get; protected set; }

        /// <summary>
        /// Gets the Content-Type header value.
        /// </summary>
        public string ContentType { get; protected set; }

        /// <summary>
        /// Gets the charset part of the Content-Type header value.
        /// </summary>
        public string ContentCharset { get; protected set; }

        /// <summary>
        /// Gets the charset part of the Content-Type header value as an <see cref="Encoding"/> object.
        /// </summary>
        public Encoding ContentCharsetEncoding { get; protected set; }

        /// <summary>
        /// Gets the Content-Language header value.
        /// </summary>
        public string ContentLanguage { get; protected set; }

        /// <summary>
        /// Gets the Content-Language header value as a <see cref="CultureInfo"/> object.
        /// </summary>
        public CultureInfo ContentLanguageCulture { get; protected set; }

        /// <summary>
        /// Gets the Content-Encoding header value.
        /// </summary>
        public string ContentEncoding { get; protected set; }

        /// <summary>
        /// Gets the Content-Length header value.
        /// </summary>
        public int ContentLength { get; protected set; }

        /// <summary>
        /// Gets the Authorization header value.
        /// </summary>
        public string Authorization { get; protected set; }

        /// <summary>
        /// Gets the Host header value.
        /// </summary>
        public string Host { get; protected set; }

        /// <summary>
        /// Gets the Referrer header value.
        /// </summary>
        public string Referrer { get; protected set; }

        /// <summary>
        /// Gets the User-Agent header value.
        /// </summary>
        public string UserAgent { get; protected set; }

        private int GetContentLength()
        {
            int contentLength;

            if (!Int32.TryParse(TryGet("Content-Length"), NumberStyles.Integer, CultureInfo.InvariantCulture, out contentLength) || contentLength < 0)
            {
                contentLength = 0;
            }

            return contentLength;
        }

        private void SetContentCharset(AcceptValueCollection contentTypes)
        {
            string contentCharset;

            if (!String.IsNullOrEmpty(ContentType) && ContentType.IndexOf('*') < 0)
            {
                contentCharset = contentTypes.GetPreferredValues(ContentType)
                                     .Where(v => v.Charset != null)
                                     .Select(v => v.Charset).FirstOrDefault() ?? Utf8Charset;
            }
            else
            {
                contentCharset = Utf8Charset;
            }

            try
            {
                ContentCharsetEncoding = Encoding.GetEncoding(contentCharset);
                ContentCharset = ContentCharsetEncoding.WebName;
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, RestResources.UnsupportedContentCharset);
            }
        }

        private void SetContentLanguage()
        {
            ContentLanguage = TryGet("Content-Language");

            if (!String.IsNullOrEmpty(ContentLanguage))
            {
                try
                {
                    ContentLanguageCulture = new CultureInfo(ContentLanguage);
                }
                catch (Exception)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest, RestResources.UnsupportedContentLanguage);
                }
            }
        }

        private void SetAcceptValues()
        {
            var acceptValues = new AcceptValueCollection(TryGet("Accept"));
            AcceptType = acceptValues.GetPreferredName();
            AcceptTypes = acceptValues.AcceptedNames;

            AcceptValue? acceptedTypeValue = acceptValues.GetPreferredValue();
            AcceptVersion = acceptedTypeValue.HasValue ? acceptedTypeValue.Value.Version : 0;
        }

        private void SetAcceptCharsets()
        {
            var acceptCharsets = new AcceptValueCollection(TryGet("Accept-Charset"));
            string acceptCharset = acceptCharsets.GetPreferredName();

            if (!String.IsNullOrEmpty(acceptCharset))
            {
                AcceptCharsets = acceptCharsets.AcceptedNames;
            }
            else
            {
                acceptCharset = Utf8Charset;
                AcceptCharsets = new[] { Utf8Charset };
            }

            try
            {
                AcceptCharsetEncoding = Encoding.GetEncoding(acceptCharset);
                AcceptCharset = AcceptCharsetEncoding.WebName;
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable, RestResources.NonAcceptedContentCharset);
            }
        }

        private void SetAcceptEncodings()
        {
            var acceptEncodings = new AcceptValueCollection(TryGet("Accept-Encoding"));
            AcceptEncoding = acceptEncodings.GetPreferredName();
            AcceptEncodings = acceptEncodings.AcceptedNames;
        }

        private void SetAcceptLanguages()
        {
            var acceptLanguages = new AcceptValueCollection(TryGet("Accept-Language"));
            AcceptLanguage = acceptLanguages.GetPreferredName();
            AcceptLanguages = acceptLanguages.AcceptedNames;

            if (String.IsNullOrEmpty(AcceptLanguage))
            {
                return;
            }

            if (AcceptLanguage == "*")
            {
                AcceptLanguageCulture = Thread.CurrentThread.CurrentCulture;
                return;
            }

            try
            {
                AcceptLanguageCulture = new CultureInfo(AcceptLanguage);
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable, RestResources.NonAcceptedContentLanguage);
            }
        }
    }
}
