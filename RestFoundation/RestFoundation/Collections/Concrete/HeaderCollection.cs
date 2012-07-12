using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using RestFoundation.Collections.Specialized;

namespace RestFoundation.Collections.Concrete
{
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
            ContentLanguage = TryGet("Content-Language");

            var contentTypes = new AcceptValueCollection(TryGet("Content-Type"));
            ContentType = contentTypes.GetPreferredName();

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
                throw new HttpResponseException(HttpStatusCode.BadRequest, "The content charset provided is not supported");
            }

            var acceptValues = new AcceptValueCollection(TryGet("Accept"));
            AcceptType = acceptValues.GetPreferredName();
            AcceptTypes = acceptValues.AcceptedNames;

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
                throw new HttpResponseException(HttpStatusCode.NotAcceptable, "No accepted charset was provided in the Accept-Charset header");
            }

            var acceptEncodings = new AcceptValueCollection(TryGet("Accept-Encoding"));
            AcceptEncoding = acceptEncodings.GetPreferredName();
            AcceptEncodings = acceptEncodings.AcceptedNames;
        }

        public string AcceptType { get; protected set; }
        public IEnumerable<string> AcceptTypes { get; protected set; }
        public string AcceptCharset { get; protected set; }
        public IEnumerable<string> AcceptCharsets { get; protected set; }
        public Encoding AcceptCharsetEncoding { get; protected set; }
        public string AcceptEncoding { get; protected set; }
        public IEnumerable<string> AcceptEncodings { get; protected set; }

        public string ContentType { get; protected set; }
        public string ContentCharset { get; protected set; }
        public Encoding ContentCharsetEncoding { get; protected set; }
        public string ContentLanguage{ get; protected set; }
        public string ContentEncoding{ get; protected set; }
        public int ContentLength { get; protected set; }

        public string Authorization { get; protected set; }
        public string Host { get; protected set; }
        public string Referrer { get; protected set; }
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
    }
}
