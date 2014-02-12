// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System.Reflection;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Contains associated objects for the "method applies" behavior.
    /// </summary>
    public class MethodAppliesContext : BehaviorContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodAppliesContext"/> class.
        /// </summary>
        /// <param name="service">The service instance.</param>
        /// <param name="method">The service method.</param>
        public MethodAppliesContext(object service, MethodInfo method) : base(service, method)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodAppliesContext"/> class.
        /// </summary>
        protected MethodAppliesContext()
        {
        }
    }
}
