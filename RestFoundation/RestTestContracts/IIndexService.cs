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
        IResult Get(int? id, string someGarbage);

        [Url("index", HttpMethod.Post)]
        Person Post(Person resource);

        [Url("index/{id}", HttpMethod.Put)]
        Person Put(int? id, Person resource);

        [Url("index/{id}", HttpMethod.Patch)]
        Person Patch(int? id, Person resource);

        [Url("index/{value}", HttpMethod.Delete)]
        ContentResult Delete();
    }
}
