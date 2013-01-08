// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System.Reflection;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Contains associated objects for the "method executing" behavior.
    /// </summary>
    public class MethodExecutingContext : BehaviorContext
    {
        private readonly object m_resource;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodExecutingContext"/> class.
        /// </summary>
        /// <param name="service">The service instance.</param>
        /// <param name="method">The service method.</param>
        /// <param name="resource">The resource for the service method.</param>
        public MethodExecutingContext(object service, MethodInfo method, object resource) : base(service, method)
        {
            m_resource = resource;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodExecutingContext"/> class.
        /// </summary>
        protected MethodExecutingContext()
        {
        }

        /// <summary>
        /// Gets the resource object to be passed into the service method.
        /// </summary>
        public virtual object Resource
        {
            get
            {
                return m_resource;
            }
        }
    }
}
