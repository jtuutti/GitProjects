using System.Reflection;

namespace RestFoundation
{
    public interface ISecureServiceBehavior : IServiceBehavior
    {
        void OnMethodAuthorizing(IServiceContext context, object service, MethodInfo method);
    }
}
