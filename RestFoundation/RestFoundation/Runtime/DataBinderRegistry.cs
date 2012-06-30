using System;
using System.Collections.Concurrent;

namespace RestFoundation.Runtime
{
    internal static class DataBinderRegistry
    {
        private static readonly ConcurrentDictionary<Type, IDataBinder> binders = new ConcurrentDictionary<Type, IDataBinder>();

        public static IDataBinder GetBinder(Type objectType)
        {
            if (objectType == null)
            {
                return null;
            }

            IDataBinder binder;

            return binders.TryGetValue(objectType, out binder) ? binder : null;
        }

        public static void SetBinder(Type objectType, IDataBinder binder)
        {
            binders.AddOrUpdate(objectType, type => binder, (type, previousFormatter) => binder);
        }

        public static bool RemoveBinder(Type objectType)
        {
            if (objectType == null)
            {
                return false;
            }

            IDataBinder binder;

            return binders.TryRemove(objectType, out binder);
        }

        public static void ClearBinders()
        {
            binders.Clear();
        }
    }
}
