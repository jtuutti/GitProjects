using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RestFoundation.Behaviors;

namespace RestFoundation.Runtime
{
    internal static class ServiceBehaviorRegistry
    {
        private static readonly List<IServiceBehavior> globalBehaviors = new List<IServiceBehavior>
        {
            new ResourceValidationBehavior()
        };

        private static readonly ConcurrentDictionary<IRestHandler, List<IServiceBehavior>> handlerBehaviors = new ConcurrentDictionary<IRestHandler, List<IServiceBehavior>>();
        private static readonly object globalSyncRoot = new object();
        private static readonly object handlerSyncRoot = new object();

        public static List<IServiceBehavior> GetBehaviors(IRestHandler handler)
        {
            var allBehaviors = new List<IServiceBehavior>(globalBehaviors);
            List<IServiceBehavior> serviceBehaviors;

            if (handlerBehaviors.TryGetValue(handler, out serviceBehaviors))
            {
                foreach (IServiceBehavior serviceBehavior in serviceBehaviors)
                {
                    TryRemoveBehavior(serviceBehavior, allBehaviors);
                    allBehaviors.Add(serviceBehavior);
                }
            }

            return allBehaviors;
        }

        public static void AddBehavior(IRestHandler handler, IServiceBehavior behavior)
        {
            handlerBehaviors.AddOrUpdate(handler,
                                         handlerToAdd =>
                                         {
                                             var behaviorsToAdd = new List<IServiceBehavior>
                                             {
                                                 behavior
                                             };

                                             return behaviorsToAdd;
                                         },
                                         (handlerToUpdate, behaviorsToUpdate) =>
                                         {
                                             lock (handlerSyncRoot)
                                             {
                                                 if (behaviorsToUpdate.Contains(behavior, ServiceBehaviorEqualityComparer.Default))
                                                 {
                                                     behaviorsToUpdate.RemoveAll(b => b.GetType() == behavior.GetType());
                                                 }

                                                 behaviorsToUpdate.Add(behavior);
                                             }

                                             return behaviorsToUpdate;
                                         });
        }

        public static void AddGlobalBehavior(IServiceBehavior behavior)
        {
            lock (globalSyncRoot)
            {
                if (!globalBehaviors.Contains(behavior, ServiceBehaviorEqualityComparer.Default))
                {
                    globalBehaviors.Add(behavior);
                }
            }
        }

        public static List<IServiceBehavior> GetGlobalBehaviors()
        {
            return new List<IServiceBehavior>(globalBehaviors);
        }

        public static bool RemoveGlobalBehavior(IServiceBehavior behavior)
        {
            return globalBehaviors.Remove(behavior);
        }

        public static void ClearGlobalBehaviors()
        {
            globalBehaviors.Clear();
        }

        private static void TryRemoveBehavior(IServiceBehavior behavior, List<IServiceBehavior> serviceBehaviors)
        {
            for (int i = serviceBehaviors.Count - 1; i >= 0; i--)
            {
                if (ServiceBehaviorEqualityComparer.Default.Equals(behavior, serviceBehaviors[i]))
                {
                    serviceBehaviors.RemoveAt(i);
                }
            }
        }

        #region Equality Comparer

        private class ServiceBehaviorEqualityComparer : IEqualityComparer<IServiceBehavior>
        {
            public static readonly ServiceBehaviorEqualityComparer Default = new ServiceBehaviorEqualityComparer();

            public bool Equals(IServiceBehavior x, IServiceBehavior y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(null, x) || ReferenceEquals(null, y))
                {
                    return false;
                }

                return x.GetType() == y.GetType();
            }

            public int GetHashCode(IServiceBehavior obj)
            {
                return obj != null ? obj.GetType().GetHashCode() : 0;
            }
        }

        #endregion
    }
}
