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
    [ProxyAdditionalHeader("Authorization", "Basic YWRtaW46UmVzdA==", ServiceRelativeUrl = "~/async")]
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
        [ProxyResourceExample(ResponseBuilderType = typeof(PersonArrayExampleBuilder))]
        IQueryable<Person> GetAll();

        [Url("index/{id}")]
        [ProxyOperationDescription("Gets resources of type 'Index' by ID")]
        ContentResult Get([ParameterConstraint(@"\d{1,3}"), ProxyRouteParameter(1)] int? id, string dummyParam);

        [Url("index")]
        [ProxyStatusCode(HttpStatusCode.Created, "Resource is created")]
        [ProxyResourceExample(RequestBuilderType = typeof(PersonExampleBuilder), ResponseBuilderType = typeof(PersonExampleBuilder))]
        object Post(Person resource);

        [Url("index/{id}")]
        [ProxyStatusCode(HttpStatusCode.OK, "Resource is updated")]
        [ProxyResourceExample(RequestBuilderType = typeof(PersonExampleBuilder), ResponseBuilderType = typeof(PersonExampleBuilder))]
        Person Put([ParameterConstraint(@"\d{1,3}"), ProxyRouteParameter(1)] int? id, [ResourceParameter] Person personToUpdate);

        [Url("index/{id}")]
        [ProxyStatusCode(HttpStatusCode.OK, "Resource is partially updated")]
        [ProxyResourceExample(RequestBuilderType = typeof(PersonExampleBuilder), ResponseBuilderType = typeof(PersonExampleBuilder))]
        Person Patch([ParameterConstraint(@"\d{1,3}"), ProxyRouteParameter(1)] int? id, Person resource);

        [Url("index/{name}")]
        [ProxyStatusCode(HttpStatusCode.NoContent, "Resource is deleted")]
        StatusResult Delete([ProxyRouteParameter("John Doe")] string name);
    }
}
