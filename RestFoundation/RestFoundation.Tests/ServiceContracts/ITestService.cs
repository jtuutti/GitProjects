namespace RestFoundation.Tests.ServiceContracts
{
    public interface ITestService
    {
        [Url("{id}", HttpMethod.Get, HttpMethod.Head)]
        IResult Get(int? id);

        [Url("all/{orderBy}", HttpMethod.Get, HttpMethod.Head)]
        IResult GetAll([ParameterConstraint("[a-zA-Z_][a-zA-Z0-9_]*")] string orderBy);

        // has a higher priority for the "new" constant to take precedence over the URLs with a dynamic parameter
        [Url("new", HttpMethod.Post, Priority = 1)]
        IResult Post();

        [Url("{id}", HttpMethod.Put)]
        IResult Put(int? id);

        [Url("{id}", HttpMethod.Patch)]
        IResult Patch(int? id);

        [Url("{id}", HttpMethod.Delete)]
        IResult Delete(int? id);
    }
}
