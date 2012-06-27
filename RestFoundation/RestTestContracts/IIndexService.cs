using System.Net;
using RestFoundation;
using RestFoundation.Results;
using RestFoundation.ServiceProxy.Attributes;
using RestTestContracts.Resources;

namespace RestTestContracts
{
    public interface IIndexService
    {
        [Url("index/{id}", HttpMethod.Get, HttpMethod.Head)]
        [ProxyOperationDescription("Gets resources of type 'Index' by ID")]
        IResult Get([ProxyRouteParameter(1)] int? id, string someGarbage);

        [Url("index", HttpMethod.Post)]
        [ProxyStatusCode(HttpStatusCode.Created, "Resource is created")]
        Person Post(Person resource);

        [Url("index/{id}", HttpMethod.Put)]
        [ProxyStatusCode(HttpStatusCode.OK, "Resource is updated")]
        Person Put([ProxyRouteParameter(1)] int? id, Person resource);

        [Url("index/{id}", HttpMethod.Patch)]
        [ProxyStatusCode(HttpStatusCode.OK, "Resource is partially updated")]
        Person Patch([ProxyRouteParameter(1)] int? id, Person resource);

        [Url("index/{name}", HttpMethod.Delete)]
        [ProxyStatusCode(HttpStatusCode.OK, "Resource is deleted")]
        ContentResult Delete([ProxyRouteParameter("John Doe")] string name);
    }
}
