using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace RestFoundation.UnitTesting
{
    internal sealed class TestHttpResponse : HttpResponseBase, IDisposable
    {
        private readonly TestHttpCachePolicyBase m_cachePolicy;
        private readonly NameValueCollection m_headers;
        private readonly HttpCookieCollection m_cookies;
        private readonly MemoryStream m_outputStream;
        private readonly StreamWriter m_output;
        private Stream m_filter;
        private bool m_isDisposed;

        internal TestHttpResponse()
        {
            m_cachePolicy = new TestHttpCachePolicyBase();
            m_headers = new NameValueCollection();
            m_cookies = new HttpCookieCollection();
            m_outputStream = new MemoryStream();
            m_output = new StreamWriter(m_outputStream, Encoding.UTF8);
            m_filter = new MemoryStream();

            m_output.AutoFlush = true;

            StatusCode = 200;
            StatusDescription = "OK";
        }

        public override string Charset { get; set; }
        public override string ContentType { get; set; }
        public override Encoding ContentEncoding { get; set; }
        public override string Status { get; set; }
        public override int StatusCode { get; set; }
        public override string StatusDescription { get; set; }
        public override bool SuppressContent { get; set; }
        public override bool TrySkipIisCustomErrors { get; set; }

        public override NameValueCollection Headers
        {
            get
            {
                return m_headers;
            }
        }

        public override HttpCookieCollection Cookies
        {
            get
            {
                return m_cookies;
            }
        }

        public override HttpCachePolicyBase Cache
        {
            get
            {
                return m_cachePolicy;
            }
        }

        public override Stream OutputStream
        {
            get
            {
                return m_outputStream;
            }
        }

        public override TextWriter Output
        {
            get
            {
                return m_output;
            }
        }

        public override Stream Filter
        {
            get
            {
                return m_filter;
            }
            set
            {
                m_filter = value;
            }
        }

        public override bool BufferOutput { get; set; }

        public override void AddHeader(string name, string value)
        {
            m_headers.Add(name, value);
        }

        public override void AppendHeader(string name, string value)
        {
            m_headers.Add(name, value);
        }

        public override void AddCacheItemDependency(string cacheKey)
        {
        }

        public override void AddCacheDependency(params CacheDependency[] dependencies)
        {
        }

        public override void AddCacheItemDependencies(ArrayList cacheKeys)
        {
        }

        public override void AddCacheItemDependencies(string[] cacheKeys)
        {
        }

        public override void AddFileDependency(string filename)
        {
        }

        public override void AddFileDependencies(string[] filenames)
        {
        }

        public override void AddFileDependencies(ArrayList filenames)
        {
        }

        public override void ClearContent()
        {
        }

        public override void ClearHeaders()
        {
            m_headers.Clear();
        }

        public override void Clear()
        {
        }

        public override void Flush()
        {
        }

        public override void AppendCookie(HttpCookie cookie)
        {
            m_cookies.Add(cookie);
        }

        public override void TransmitFile(string filename)
        {
        }

        public override void TransmitFile(string filename, long offset, long length)
        {
        }

        public override void Write(char ch)
        {
            Console.Write(ch);
        }

        public override void Write(object obj)
        {
            Console.Write(obj);
        }

        public override void Write(string s)
        {
            Console.Write(s);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            Console.Write(buffer, index, count);
        }

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "m_output",
                         Justification = "Disposing the stream will close the writer")]
        public void Dispose()
        {
            if (m_isDisposed)
            {
                return;
            }

            m_outputStream.Dispose();
            m_filter.Dispose();

            m_isDisposed = true;
        }
    }
}
