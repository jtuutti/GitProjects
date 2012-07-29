using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RestFoundation.Behaviors;
using RestFoundation.Runtime;

namespace RestFoundation
{
    /// <summary>
    /// Represents a global behavior builder.
    /// </summary>
    public sealed class GlobalBehaviorBuilder
    {
        internal GlobalBehaviorBuilder()
        {
        }

        /// <summary>
        /// Adds the provided global behaviors.
        /// </summary>
        /// <param name="behaviors">An array of behavior instances.</param>
        public void AddGlobalBehaviors(params IServiceBehavior[] behaviors)
        {
            if (behaviors == null)
            {
                throw new ArgumentNullException("behaviors");
            }

            if (behaviors.GroupBy(s => s.GetType()).Max(g => g.Count()) > 1)
            {
                throw new InvalidOperationException("Multiple global service behaviors of the same type are not allowed");
            }

            for (int i = 0; i < behaviors.Length; i++)
            {
                ServiceBehaviorRegistry.AddGlobalBehavior(behaviors[i]);
            }
        }

        /// <summary>
        /// Gets all registered global behaviors.
        /// </summary>
        /// <returns>A sequence of behavior instances.</returns>
        public IEnumerable<IServiceBehavior> GetGlobalBehaviors()
        {
            return new ReadOnlyCollection<IServiceBehavior>(ServiceBehaviorRegistry.GetGlobalBehaviors());
        }

        /// <summary>
        /// Removes/unregisters a global behavior instance.
        /// </summary>
        /// <param name="behavior">The behavior instance.</param>
        /// <returns>true if the behavior was removed; false if the behavior instance had not been registered</returns>
        public bool RemoveGlobalBehavior(IServiceBehavior behavior)
        {
            if (behavior == null)
            {
                throw new ArgumentNullException("behavior");
            }

            return ServiceBehaviorRegistry.RemoveGlobalBehavior(behavior);
        }

        /// <summary>
        /// Clears/unregisters all global behaviors.
        /// </summary>
        public void ClearGlobalBehaviors()
        {
            ServiceBehaviorRegistry.ClearGlobalBehaviors();
        }
    }
}
