using System;
using System.Net;
using System.Text;
using System.Web;

namespace RestFoundation
{
    public interface IHttpResponse
    {
        IHttpResponseOutput Output { get; }
        bool TrySkipIisCustomErrors { get; set; }

        string GetHeader(string headerName);
        void SetHeader(string headerName, string headerValue);
        bool RemoveHeader(string headerName);
        void ClearHeaders();

        void SetCharsetEncoding(Encoding encoding);

        HttpStatusCode GetStatusCode();
        string GetStatusDescription();
        void SetStatus(HttpStatusCode statusCode);
        void SetStatus(HttpStatusCode statusCode, string statusDescription);

        HttpCookie GetCookie(string cookieName);
        void SetCookie(HttpCookie cookie);
        void ExpireCookie(HttpCookie cookie);

        void SetFileDependencies(string filePath);
        void SetFileDependencies(string filePath, TimeSpan maxAge);
        void SetFileDependencies(string filePath, HttpCacheability cacheability, TimeSpan maxAge);
        void TransmitFile(string filePath);
    }
}
