using RestFoundation.Runtime;

namespace RestFoundation
{
    public interface IServiceMethodLocator
    {
        ServiceMethodLocatorData Execute(IRestHandler handler);
    }
}
