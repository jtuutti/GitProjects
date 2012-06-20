using System.Collections.Generic;
using System.Web;

namespace RestFoundation.Collections
{
    public interface ICookieValueCollection : IEnumerable<HttpCookie>
    {
        ICollection<string> Keys { get; }
        int Count { get; }

        HttpCookie Get(string key);
        HttpCookie TryGet(string key);
    }
}
