using System.Collections.Generic;
using System.Collections.Specialized;

namespace RestFoundation.Collections
{
    public interface IStringValueCollection : IEnumerable<string>
    {
        ICollection<string> Keys { get; }
        int Count { get; }

        string Get(string key);
        string TryGet(string key);

        ICollection<string> GetValues(string key);

        NameValueCollection ToNameValueCollection();
    }
}
