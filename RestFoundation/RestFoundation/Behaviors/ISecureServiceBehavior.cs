// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
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
        /// <param name="serviceContext">The service context.</param>
        /// <param name="behaviorContext">The "method authorizing" behavior context.</param>
        void OnMethodAuthorizing(IServiceContext serviceContext, MethodAuthorizingContext behaviorContext);
    }
}
