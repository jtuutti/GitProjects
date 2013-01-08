// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Defines a special case of a <see cref="ISecureServiceBehavior"/> that must be invoked
    /// before any other behaviors. Only a single <see cref="IAuthenticationBehavior"/> can be
    /// associated with a service method.
    /// </summary>
    public interface IAuthenticationBehavior : ISecureServiceBehavior
    {
    }
}
