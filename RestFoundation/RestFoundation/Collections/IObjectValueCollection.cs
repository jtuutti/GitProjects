using System.Collections.Generic;

namespace RestFoundation.Collections
{
    public interface IObjectValueCollection : IEnumerable<object>
    {
        ICollection<string> Keys { get; }
        int Count { get; }

        object Get(string key);
        object TryGet(string key);

        IDictionary<string, object> ToDictionary();
    }
}
