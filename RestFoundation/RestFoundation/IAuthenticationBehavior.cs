namespace RestFoundation
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
