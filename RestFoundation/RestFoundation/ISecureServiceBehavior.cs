using System.Reflection;

namespace RestFoundation
{
    public interface ISecureServiceBehavior : IServiceBehavior
    {
        void OnMethodAuthorizing(object service, MethodInfo method);
    }
}
