using RestFoundation.Results;
using RestFoundation.Tests.Implementation.Models;

namespace RestFoundation.Tests.Implementation.ServiceContracts
{
    public interface ITestService
    {
        [Url("{id}")]
        IResult Get(int? id);

        [Url("all/{orderBy}")]
        IResult GetAll([ParameterConstraint("[a-zA-Z_][a-zA-Z0-9_]*")] string orderBy);

        // has a higher priority for the "new" constant to take precedence over the URLs with a dynamic parameter
        [Url("new", Priority = 1)]
        IResult Post(Model resource);

        [Url("{id}")]
        IResult Put(int? id);

        [Url("{id}")]
        IResult Patch(int? id);

        [Url("{id}")]
        void Delete(int? id);
    }
}
