using System.Reflection;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Defines a secure service behavior.
    /// </summary>
    public interface ISecureServiceBehavior : IServiceBehavior
    {
        /// <summary>
        /// Called during the authorization process before a service method or behavior is executed.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="service">The service object.</param>
        /// <param name="method">The service method.</param>
        void OnMethodAuthorizing(IServiceContext context, object service, MethodInfo method);
    }
}
