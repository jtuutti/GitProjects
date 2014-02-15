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
    /// <summary>
    /// Defines the main service.
    /// </summary>
    [ProxyMetadata(typeof(IndexServiceMetadata))]
    public interface IIndexService
    {
        /// <summary>
        /// Redirects to the resource with the provided ID.
        /// </summary>
        /// <param name="id">The resource ID.</param>
        /// <returns>The redirect result.</returns>
        [Url("index/{id}/with-redirect", HttpMethod.Get)]
        RedirectResult RedirectToGet(int id);

        /// <summary>
        /// Returns a dynamic dictionary.
        /// </summary>
        /// <returns>The dynamic data dictionary.</returns>
        [Url("dyn")]
        dynamic GetDynamicDict();

        /// <summary>
        /// Gets all the person records.
        /// </summary>
        /// <returns>The collection of person records.</returns>
        [Url("index/all")]
        IQueryable<Person> GetAll();

        /// <summary>
        /// Gets all the person records asynchronously.
        /// </summary>
        /// <returns>The collection of person records.</returns>
        [Url("index/all-async")]
        Task<IQueryable<Person>> GetAllAsync();

        /// <summary>
        /// Gets all the person records in HTTP chunks.
        /// </summary>
        /// <returns>The collection of person records.</returns>
        [Url("index/all-chunked")]
        IEnumerable<Person> GetAllChunked();

        /// <summary>
        /// Gets all the person records in the specified media type format.
        /// </summary>
        /// <returns>The collection of person records.</returns>
        [Url("index/all.{format}")]
        IResult GetAllByFormat(string format);

        /// <summary>
        /// Gets HTML content using an optional ID (default is 1) and an optional dummy string.
        /// </summary>
        /// <param name="id">The resource id.</param>
        /// <param name="dummy">The dummy string.</param>
        /// <returns>The HTML content.</returns>
        [Url("index/{id}")]
        ContentResult Get([Constraint(Constraint.UnsignedInteger)] int? id = 1, [FromUri] string dummy = "N/A");

        /// <summary>
        /// Inserts a new person resource.
        /// </summary>
        /// <param name="resource">The person resource.</param>
        /// <returns>The inserted person resource.</returns>
        [Url("index"), AssertValidation(true)]
        object Post(Person resource);

        /// <summary>
        /// Updates a person resource.
        /// </summary>
        /// <param name="id">The resource id.</param>
        /// <param name="personToUpdate">The person resource.</param>
        /// <returns>The updated person resource.</returns>
        [Url("index/{id}"), AssertValidation(true)]
        Person Put([Constraint(Constraint.UnsignedInteger)] int? id, [Resource] Person personToUpdate);

        /// <summary>
        /// Updates partial data in a person resource.
        /// </summary>
        /// <param name="id">The resource id.</param>
        /// <param name="resource">The person resource.</param>
        /// <returns>The updated person resource.</returns>
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

        /// <summary>
        /// Generates a person resource from the POSTed form name/value parameters.
        /// </summary>
        /// <param name="name">The person's name.</param>
        /// <param name="age">The person's age.</param>
        /// <param name="timestamp">An optional timestamp.</param>
        /// <returns>The inserted person resource.</returns>
        [Url("index/form-data", Priority = 10 /* needs to take precedence over the parameterized methods */)]
        Person PostMultipleParameters([FromBody] string name, [FromBody] int age, [FromBody] DateTime? timestamp);
    }
}
