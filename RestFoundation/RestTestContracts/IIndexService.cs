using System.Linq;
using System.Net;
using RestFoundation;
using RestFoundation.Results;
using RestFoundation.ServiceProxy.Attributes;
using RestTestContracts.Metadata;
using RestTestContracts.Resources;

namespace RestTestContracts
{
    public interface IIndexService
    {
        [Url("index/all", HttpMethod.Get, HttpMethod.Head)]
        [ProxyOperationDescription("Gets all resources of type 'Index'")]
        [ProxyResourceExample(ResponseExampleType = typeof(PersonArrayExample))]
        IQueryable<Person> GetAll();

        [Url("index/{id}", HttpMethod.Get, HttpMethod.Head)]
        [ProxyOperationDescription("Gets resources of type 'Index' by ID")]
        ContentResult Get([ParameterConstraint(@"\d{1,3}"), ProxyRouteParameter(1)] int? id, string dummyParam);

        [Url("index", HttpMethod.Post)]
        [ProxyStatusCode(HttpStatusCode.Created, "Resource is created")]
        [ProxyResourceExample(RequestExampleType = typeof(PersonExample), ResponseExampleType = typeof(PersonExample))]
        Person Post(Person resource);

        [Url("index/{id}", HttpMethod.Put)]
        [ProxyStatusCode(HttpStatusCode.OK, "Resource is updated")]
        [ProxyResourceExample(RequestExampleType = typeof(PersonExample), ResponseExampleType = typeof(PersonExample))]
        Person Put([ParameterConstraint(@"\d{1,3}"), ProxyRouteParameter(1)] int? id, [BindResource] Person personToUpdate);

        [Url("index/{id}", HttpMethod.Patch)]
        [ProxyStatusCode(HttpStatusCode.OK, "Resource is partially updated")]
        [ProxyResourceExample(RequestExampleType = typeof(PersonExample), ResponseExampleType = typeof(PersonExample))]
        Person Patch([ParameterConstraint(@"\d{1,3}"), ProxyRouteParameter(1)] int? id, Person resource);

        [Url("index/{name}", HttpMethod.Delete)]
        [ProxyStatusCode(HttpStatusCode.NoContent, "Resource is deleted")]
        StatusResult Delete([ProxyRouteParameter("John Doe")] string name);
    }
}
