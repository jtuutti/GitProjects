using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RestFoundation.Runtime;

namespace RestFoundation
{
    public sealed class BehaviorBuilder
    {
        internal BehaviorBuilder()
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
                BehaviorRegistry.AddGlobalBehavior(behaviors[i]);
            }
        }

        public IEnumerable<IServiceBehavior> GetGlobalBehaviors()
        {
            return new ReadOnlyCollection<IServiceBehavior>(BehaviorRegistry.GetGlobalBehaviors());
        }

        public bool RemoveGlobalBehavior(IServiceBehavior behavior)
        {
            if (behavior == null) throw new ArgumentNullException("behavior");

            return BehaviorRegistry.RemoveGlobalBehavior(behavior);
        }

        public void ClearGlobalBehaviors()
        {
            BehaviorRegistry.ClearGlobalBehaviors();
        }
    }
}
