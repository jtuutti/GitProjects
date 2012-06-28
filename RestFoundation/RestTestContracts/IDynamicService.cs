using RestFoundation;

namespace RestTestContracts
{
    public interface IDynamicService
    {
        [Url("", HttpMethod.Post)]
        dynamic Post(dynamic resource);
    }
}
