using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestFoundation;
using RestFoundation.Behaviors;
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
        [Url("index/{id}/with-redirect", HttpMethod.Get)]
        RedirectResult RedirectToGet(int id);

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
        ContentResult Get([Constraint(Constraint.UnsignedInteger)] int? id = 1, [FromUri] string dummy = "N/A");

        [Url("index"), AssertValidation(true)]
        object Post(Person resource);

        [Url("index/{id}"), AssertValidation(true)]
        Person Put([Constraint(Constraint.UnsignedInteger)] int? id, [Resource] Person personToUpdate);

        [Url("index/{id}"), AssertValidation(true)]
        Person Patch([Constraint(Constraint.UnsignedInteger)] int? id, Person resource);

        /// <summary>
        /// Deletes a contact by the name.
        /// </summary>
        /// <remarks>
        /// This is a truely RESTful method that uses URL to identify the resource and a proper HTTP method.
        /// </remarks>
        /// <param name="name">The contact name.</param>
        /// <returns>The status code.</returns>
        [Url("index/{name}")]
        StatusCodeResult Delete(string name);

        [Url("index/form-data", Priority = 10 /* need to be above the parameterized methods */)]
        Person PostMultipleParameters([FromBody] string name, [FromBody] int age, [FromBody] DateTime? timestamp);
    }
}
