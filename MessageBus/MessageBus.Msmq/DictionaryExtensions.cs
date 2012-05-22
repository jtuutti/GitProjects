using System;
using System.Collections.Generic;

namespace MessageBus.Msmq
{
    internal static class DictionaryExtensions
    {
        public static string Get(this IDictionary<string, string> dictionary, string key)
        {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            string value;
            return dictionary.TryGetValue(key, out value) ? value : null;
        }
    }
}
