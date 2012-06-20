using RestFoundation;
using RestFoundation.Results;
using RestTestContracts.Resources;

namespace RestTestContracts
{
    public interface IIndexService
    {
        [Url("", HttpMethod.Get, HttpMethod.Head)]
        [Url("index/{id}", HttpMethod.Get, HttpMethod.Head)]
        ContentResult Get(int? ID, string someGarbage);

        [Url("index", HttpMethod.Post)]
        Person Post(Person resource);

        [Url("index/{id}", HttpMethod.Put)]
        StatusResult Put();

        [Url("index/{value}", HttpMethod.Delete)]
        RedirectResult Delete();
    }
}
