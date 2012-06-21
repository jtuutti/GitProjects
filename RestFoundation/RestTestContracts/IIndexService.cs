using RestFoundation;
using RestFoundation.Results;
using RestFoundation.ServiceProxy.Attributes;
using RestTestContracts.Resources;

namespace RestTestContracts
{
    public interface IIndexService
    {
        [Url("index/{id}", HttpMethod.Get, HttpMethod.Head)]
        [UrlMetadata("Gets resources of type 'Index' by ID")]
        IResult Get(int? ID, string someGarbage);

        [Url("index", HttpMethod.Post)]
        Person Post(Person resource);

        [Url("index/{id}", HttpMethod.Put)]
        StatusResult Put();

        [Url("index/{value}", HttpMethod.Delete)]
        RedirectResult Delete();
    }
}
