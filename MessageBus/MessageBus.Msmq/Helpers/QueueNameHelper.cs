using System;
using System.Collections.Concurrent;
using System.Linq;

namespace MessageBus.Msmq.Helpers
{
    internal static class QueueNameHelper
    {
        private static readonly ConcurrentDictionary<Type, string> cachedNames = new ConcurrentDictionary<Type, string>();

        public static string GetName(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            string name;

            if (!cachedNames.TryGetValue(type, out name))
            {
                var nameAttribute = type.GetCustomAttributes(typeof(QueueNameAttribute), true).OfType<QueueNameAttribute>().SingleOrDefault();
                name = nameAttribute != null ? nameAttribute.Name : type.Name;

                cachedNames.TryAdd(type, name);
            }

            return name;
        }
    }
}
