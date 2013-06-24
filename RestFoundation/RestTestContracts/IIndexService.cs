using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestFoundation;
using RestFoundation.Behaviors.Attributes;
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
        [Url("index/get-10", HttpMethod.Get)]
        RedirectResult RedirectToGet10();

        [Url("dyn")]
        dynamic GetDynamicDict();

        [Url("index/all")]
        IQueryable<Person> GetAll();

        [Url("index/all-async")]
        Task<IQueryable<Person>> GetAllAsync();

        [Url("index/all-chunked")]
        IEnumerable<Person> GetAllChunked();

        [Url("index/all.{format}")]
        IResult GetAllByFormat(string format);

        [Url("index/{id}")]
        ContentResult Get([Constraint(ParameterType.UnsignedInteger)] int? id = 1, [FromUri] string dummy = "N/A");

        [Url("index"), AssertValidation(true)]
        object Post(Person resource);

        [Url("index/{id}"), AssertValidation(true)]
        Person Put([Constraint(ParameterType.UnsignedInteger)] int? id, [Resource] Person personToUpdate);

        [Url("index/{id}"), AssertValidation(true)]
        Person Patch([Constraint(ParameterType.UnsignedInteger)] int? id, Person resource);

        [Url("index/{name}")]
        StatusResult Delete(string name);

        [Url("index/form-data", Priority = 10 /* need to be above the parameterized methods */)]
        Person PostMultipleParameters([FromBody] string name, [FromBody] int age, [FromBody] DateTime? timestamp);
    }
}
