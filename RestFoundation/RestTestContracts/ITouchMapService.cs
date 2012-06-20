using RestFoundation;

namespace RestTestContracts
{
    public interface ITouchMapService
    {
        [Url("", HttpMethod.Get, HttpMethod.Head)]
        object Get();
    }
}
