// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RestFoundation.Behaviors;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation.Runtime
{
    internal static class ServiceBehaviorRegistry
    {
        private static readonly List<IServiceBehavior> globalBehaviors = new List<IServiceBehavior>
        {
            new ResourceValidationBehavior()
        };

        private static readonly ConcurrentDictionary<IRestServiceHandler, List<IServiceBehavior>> handlerBehaviors = new ConcurrentDictionary<IRestServiceHandler, List<IServiceBehavior>>();
        private static readonly object globalSyncRoot = new object();
        private static readonly object handlerSyncRoot = new object();

        public static IList<IServiceBehavior> GetBehaviors(IRestServiceHandler handler)
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

            PrioritizeAuthenticationBehavior(allBehaviors);

            return allBehaviors;
        }

        public static void AddBehavior(IRestServiceHandler handler, IServiceBehavior behavior)
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

        public static IList<IServiceBehavior> GetGlobalBehaviors()
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

        private static void TryRemoveBehavior(IServiceBehavior behavior, List<IServiceBehavior> behaviors)
        {
            for (int i = behaviors.Count - 1; i >= 0; i--)
            {
                if (ServiceBehaviorEqualityComparer.Default.Equals(behavior, behaviors[i]))
                {
                    behaviors.RemoveAt(i);
                }
            }
        }

        private static void PrioritizeAuthenticationBehavior(List<IServiceBehavior> behaviors)
        {
            int authBehaviorIndex = behaviors.FindIndex(b => b is IAuthenticationBehavior);

            if (authBehaviorIndex < 0)
            {
                return;
            }

            int lastAuthBehaviorIndex = behaviors.FindLastIndex(b => b is IAuthenticationBehavior);

            if (authBehaviorIndex != lastAuthBehaviorIndex)
            {
                throw new InvalidOperationException(Resources.Global.DuplicateAuthenticationBehavior);
            }

            if (authBehaviorIndex == 0)
            {
                return;
            }

            var authBehavior = behaviors[authBehaviorIndex] as IAuthenticationBehavior;

            if (authBehavior == null)
            {
                return;
            }

            behaviors.RemoveAt(authBehaviorIndex);
            behaviors.Insert(0, authBehavior);
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
