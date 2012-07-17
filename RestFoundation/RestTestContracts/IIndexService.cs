using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestFoundation;
using RestFoundation.Results;
using RestFoundation.ServiceProxy;
using RestTestContracts.Metadata;
using RestTestContracts.Resources;

namespace RestTestContracts
{
    public interface IIndexService
    {
        [Url("index/feed.{format}", HttpMethod.Get, HttpMethod.Head)]
        [ProxyHiddenOperation]
        FeedResult Feed(string format);

        [Url("index/download", HttpMethod.Get)]
        [ProxyHiddenOperation]
        FileResultBase FileDownload();

        [Url("index/upload", HttpMethod.Post)]
        [ProxyHiddenOperation]
        ContentResult FileUpload(ICollection<IUploadedFile> files);

        [Url("index/all", "Get, Head")]
        [ProxyOperationDescription("Gets all resources of type 'Index'")]
        [ProxyResourceExample(ResponseExampleType = typeof(PersonArrayExampleBuilder))]
        IQueryable<Person> GetAll();

        [Url("index/{id}")]
        [ProxyOperationDescription("Gets resources of type 'Index' by ID")]
        ContentResult Get([ParameterConstraint(@"\d{1,3}"), ProxyRouteParameter(1)] int? id, string dummyParam);

        [Url("index")]
        [ProxyStatusCode(HttpStatusCode.Created, "Resource is created")]
        [ProxyResourceExample(RequestExampleType = typeof(PersonExampleBuilder), ResponseExampleType = typeof(PersonExampleBuilder))]
        object Post(Person resource);

        [Url("index/{id}")]
        [ProxyStatusCode(HttpStatusCode.OK, "Resource is updated")]
        [ProxyResourceExample(RequestExampleType = typeof(PersonExampleBuilder), ResponseExampleType = typeof(PersonExampleBuilder))]
        Person Put([ParameterConstraint(@"\d{1,3}"), ProxyRouteParameter(1)] int? id, [ResourceParameter] Person personToUpdate);

        [Url("index/{id}")]
        [ProxyStatusCode(HttpStatusCode.OK, "Resource is partially updated")]
        [ProxyResourceExample(RequestExampleType = typeof(PersonExampleBuilder), ResponseExampleType = typeof(PersonExampleBuilder))]
        Person Patch([ParameterConstraint(@"\d{1,3}"), ProxyRouteParameter(1)] int? id, Person resource);

        [Url("index/{name}")]
        [ProxyStatusCode(HttpStatusCode.NoContent, "Resource is deleted")]
        StatusResult Delete([ProxyRouteParameter("John Doe")] string name);
    }
}
