using RestFoundation;

namespace RestTestContracts
{
    public interface ITouchMapService
    {
        [Url(Url.Root)]
        object Get();
    }
}
