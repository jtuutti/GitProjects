using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace RestFoundation
{
    public interface IHttpResponse
    {
        Stream Output { get; }
        TextWriter OutputWriter { get; }
        Stream OutputFilter { get; set; }

        bool TrySkipIisCustomErrors { get; set; }

        string GetHeader(string headerName);
        void SetHeader(string headerName, string headerValue);
        void RemoveHeader(string headerName);

        HttpStatusCode GetStatusCode();
        string GetStatusDescription();
        void SetStatus(HttpStatusCode statusCode);
        void SetStatus(HttpStatusCode statusCode, string statusDescription);

        void SetCharsetEncoding(Encoding encoding);

        HttpCookie GetCookie(string cookieName);
        void SetCookie(HttpCookie cookie);
        void RemoveCookie(HttpCookie cookie);

        object GetHttpItem(string name);
        void SetHttpItem(string name, object value);
        void RemoveHttpItem(string name);

        void Flush();
        void Clear();
        void ClearHeaders();

        void Redirect(string url);
        void Redirect(string url, bool isPermanent);
        void Redirect(string url, bool isPermanent, bool endResponse);

        string MapPath(string filePath);

        void SetFileDependencies(string filePath);
        void SetFileDependencies(string filePath, HttpCacheability cacheability);
        void TransmitFile(string filePath);

        IHttpResponse Write(string value);
        IHttpResponse Write(object obj);
        IHttpResponse WriteLine();
        IHttpResponse WriteLine(string value);
        IHttpResponse WriteLine(byte times);
        IHttpResponse WriteFormat(string format, params object[] values);
        IHttpResponse WriteFormat(CultureInfo provider, string format, params object[] values);
    }
}
