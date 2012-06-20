using System.Collections.Generic;
using System.Text;

namespace RestFoundation.Collections
{
    public interface IHeaderCollection : IStringValueCollection
    {
        string AcceptType { get; }
        IEnumerable<string> AcceptTypes { get; }

        string AcceptCharset { get; }
        IEnumerable<string> AcceptCharsets { get; }
        Encoding AcceptCharsetEncoding { get; }

        string AcceptEncoding { get; }
        IEnumerable<string> AcceptEncodings { get; }

        string ContentType { get; }
        string ContentCharset { get; }
        Encoding ContentCharsetEncoding { get; }
        string ContentLanguage { get; }
        string ContentEncoding { get; }
        int ContentLength { get; }

        string Authorization { get; }
        string Host { get; }
        string Referrer { get; }
        string UserAgent { get; }
    }
}
