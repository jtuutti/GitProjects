using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace RestFoundation.Runtime
{
    internal static class BehaviorRegistry
    {
        private static readonly List<IServiceBehavior> globalBehaviors = new List<IServiceBehavior>();
        private static readonly ConcurrentDictionary<IRouteHandler, List<IServiceBehavior>> behaviors = new ConcurrentDictionary<IRouteHandler, List<IServiceBehavior>>();
        private static readonly object syncRoot = new object();

        public static List<IServiceBehavior> GetBehaviors(IRouteHandler routeHandler, IHttpRequest request, IHttpResponse response)
        {
            var allBehaviors = new List<IServiceBehavior>(globalBehaviors);
            List<IServiceBehavior> serviceBehaviors;

            if (behaviors.TryGetValue(routeHandler, out serviceBehaviors))
            {
                foreach (IServiceBehavior serviceBehavior in serviceBehaviors)
                {
                    TryRemoveBehavior(serviceBehavior, allBehaviors);
                    allBehaviors.Add(serviceBehavior);
                }
            }

            foreach (IServiceBehavior behavior in allBehaviors)
            {
                behavior.Request = request;
                behavior.Response = response;
            }

            return allBehaviors;
        }

        public static void AddBehavior(IRouteHandler routeHandler, IServiceBehavior behavior)
        {
            behaviors.AddOrUpdate(routeHandler,
                                  routeHandlerToAdd =>
                                  {
                                      var behaviorsToAdd = new List<IServiceBehavior>
                                                           {
                                                               behavior
                                                           };

                                      return behaviorsToAdd;
                                  },
                                  (routeHandlerToUpdate, behaviorsToUpdate) =>
                                  {
                                      if (!behaviorsToUpdate.Contains(behavior, ServiceBehaviorEqualityComparer.Default))
                                      {
                                          behaviorsToUpdate.Add(behavior);
                                      }

                                      return behaviorsToUpdate;
                                  });
        }

        public static void AddGlobalBehavior(IServiceBehavior behavior)
        {
            lock (syncRoot)
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
