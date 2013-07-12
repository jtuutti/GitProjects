// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public static IEnumerable<ITypeBinder> GetBinders()
        {
            foreach (var binderRegistration in binders)
            {
                yield return binderRegistration.Value;
            }
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

        public static bool RemoveBinder(ITypeBinder binder)
        {
            if (binder == null)
            {
                throw new ArgumentNullException("binder");
            }

            var objectTypes = new List<Type>();

            foreach (var binderRegistration in binders)
            {
                if (binderRegistration.Value == binder)
                {
                    objectTypes.Add(binderRegistration.Key);
                }
            }

            ITypeBinder tempValue = null;

            foreach (Type objectType in objectTypes)
            {
                binders.TryRemove(objectType, out tempValue);
            }

            return tempValue != null;
        }

        public static void ClearBinders()
        {
            binders.Clear();
        }
    }
}
