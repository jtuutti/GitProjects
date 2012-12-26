// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Concurrent;
using RestFoundation.TypeBinders;

namespace RestFoundation.Runtime
{
    internal static class TypeBinderRegistry
    {
        private static readonly ConcurrentDictionary<Type, ITypeBinder> binders = new ConcurrentDictionary<Type, ITypeBinder>();

        public static ITypeBinder GetBinder(Type objectType)
        {
            if (objectType == null)
            {
                return null;
            }

            ITypeBinder binder;

            return binders.TryGetValue(objectType, out binder) ? binder : null;
        }

        public static void SetBinder(Type objectType, ITypeBinder binder)
        {
            binders.AddOrUpdate(objectType, type => binder, (type, previousFormatter) => binder);
        }

        public static bool RemoveBinder(Type objectType)
        {
            if (objectType == null)
            {
                return false;
            }

            ITypeBinder binder;

            return binders.TryRemove(objectType, out binder);
        }

        public static void ClearBinders()
        {
            binders.Clear();
        }
    }
}
