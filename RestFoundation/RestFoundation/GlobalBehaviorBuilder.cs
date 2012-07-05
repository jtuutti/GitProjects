using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RestFoundation.Runtime;

namespace RestFoundation
{
    public sealed class GlobalBehaviorBuilder
    {
        internal GlobalBehaviorBuilder()
        {
        }

        public void AddGlobalBehaviors(params IServiceBehavior[] behaviors)
        {
            if (behaviors == null) throw new ArgumentNullException("behaviors");

            if (behaviors.GroupBy(s => s.GetType()).Max(g => g.Count()) > 1)
            {
                throw new InvalidOperationException("Multiple global service behaviors of the same type are not allowed");
            }

            for (int i = 0; i < behaviors.Length; i++)
            {
                ServiceBehaviorRegistry.AddGlobalBehavior(behaviors[i]);
            }
        }

        public IEnumerable<IServiceBehavior> GetGlobalBehaviors()
        {
            return new ReadOnlyCollection<IServiceBehavior>(ServiceBehaviorRegistry.GetGlobalBehaviors());
        }

        public bool RemoveGlobalBehavior(IServiceBehavior behavior)
        {
            if (behavior == null) throw new ArgumentNullException("behavior");

            return ServiceBehaviorRegistry.RemoveGlobalBehavior(behavior);
        }

        public void ClearGlobalBehaviors()
        {
            ServiceBehaviorRegistry.ClearGlobalBehaviors();
        }
    }
}
