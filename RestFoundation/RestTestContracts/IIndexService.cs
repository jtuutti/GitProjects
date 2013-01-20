using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestFoundation;
using RestFoundation.Results;
using RestFoundation.ServiceProxy;
using RestFoundation.TypeBinders;
using RestTestContracts.Metadata;
using RestTestContracts.Resources;

namespace RestTestContracts
{
    [ProxyMetadata(typeof(IndexServiceMetadata))]
    public interface IIndexService
    {
        [Url("index/feed.{format}", HttpMethod.Get, HttpMethod.Head)]
        FeedResult Feed(string format);

        [Url("index/download", HttpMethod.Get)]
        FileResultBase FileDownload([FromUri] string fileName);

        [Url("index/upload", HttpMethod.Post)]
        ContentResult FileUpload(ICollection<IUploadedFile> files);

        [Url("index/get-10", HttpMethod.Get)]
        RedirectResult RedirectToGet10();

        [Url("dyn")]
        dynamic GetDynamicDict();

        [Url("index/all", "Get, Head")]
        IQueryable<Person> GetAll();

        [Url("index/all-async", "Get, Head")]
        Task<IEnumerable<Person>> GetAllAsync();

        [Url("index/all.{format}", "Get, Head")]
        IResult GetAllByFormat(string format);

        [Url("index/{id}")]
        ContentResult Get(int? id = 1, [FromUri] string dummy = "N/A");

        [Url("index")]
        object Post(Person resource);

        [Url("index/{id}")]
        Person Put([ParameterConstraint(@"\d{1,3}")] int? id, [Resource] Person personToUpdate);

        [Url("index/{id}")]
        Person Patch([ParameterConstraint(@"\d{1,3}")] int? id, Person resource);

        [Url("index/{name}")]
        StatusResult Delete(string name);

        [Url("index/form-data")]
        Person PostMultipleParameters([FromBody] string name, [FromBody] int age, [FromBody] DateTime? timestamp);
    }
}
