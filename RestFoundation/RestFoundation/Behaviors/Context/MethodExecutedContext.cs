// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System.Reflection;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Contains associated objects for the "method executed" behavior.
    /// </summary>
    public class MethodExecutedContext : BehaviorContext
    {
        private readonly object m_returnedObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodExecutedContext"/> class.
        /// </summary>
        /// <param name="service">The service instance.</param>
        /// <param name="method">The service method.</param>
        /// <param name="returnedObject">The object returned by the service method.</param>
        public MethodExecutedContext(object service, MethodInfo method, object returnedObject) : base(service, method)
        {
            m_returnedObject = returnedObject;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodExecutedContext"/> class.
        /// </summary>
        protected MethodExecutedContext()
        {
        }

        /// <summary>
        /// Gets the object returned by the service method.
        /// </summary>
        public virtual object ReturnedObject
        {
            get
            {
                return m_returnedObject;
            }
        }
    }
}
