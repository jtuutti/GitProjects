using System;
using System.Collections.Concurrent;

namespace RestFoundation.Runtime
{
    internal static class ObjectTypeBinderRegistry
    {
        private static readonly ConcurrentDictionary<Type, IObjectTypeBinder> binders = new ConcurrentDictionary<Type, IObjectTypeBinder>();

        public static IObjectTypeBinder GetBinder(Type objectType)
        {
            if (objectType == null)
            {
                return null;
            }

            IObjectTypeBinder binder;

            return binders.TryGetValue(objectType, out binder) ? binder : null;
        }

        public static void SetBinder(Type objectType, IObjectTypeBinder binder)
        {
            binders.AddOrUpdate(objectType, type => binder, (type, previousFormatter) => binder);
        }

        public static bool RemoveBinder(Type objectType)
        {
            if (objectType == null)
            {
                return false;
            }

            IObjectTypeBinder binder;

            return binders.TryRemove(objectType, out binder);
        }

        public static void ClearBinders()
        {
            binders.Clear();
        }
    }
}
