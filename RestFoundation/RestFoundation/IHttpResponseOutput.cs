using System.Globalization;
using System.IO;

namespace RestFoundation
{
    public interface IHttpResponseOutput
    {
        Stream Stream { get; }
        TextWriter Writer { get; }
        Stream Filter { get; set; }

        void Flush();
        void Clear();

        IHttpResponseOutput Write(string value);
        IHttpResponseOutput Write(object obj);
        IHttpResponseOutput WriteLine();
        IHttpResponseOutput WriteLine(string value);
        IHttpResponseOutput WriteLine(byte times);
        IHttpResponseOutput WriteFormat(string format, params object[] values);
        IHttpResponseOutput WriteFormat(CultureInfo provider, string format, params object[] values);
    }
}
