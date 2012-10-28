// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System.Reflection;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Contains associated objects for the "method authorizing" behavior.
    /// </summary>
    public class MethodAuthorizingContext : BehaviorContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodAuthorizingContext"/> class.
        /// </summary>
        /// <param name="service">The service instance.</param>
        /// <param name="method">The service method.</param>
        public MethodAuthorizingContext(object service, MethodInfo method) : base(service, method)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodAuthorizingContext"/> class.
        /// </summary>
        protected MethodAuthorizingContext()
        {
        }
    }
}
