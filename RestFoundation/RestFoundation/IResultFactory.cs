namespace RestFoundation
{
    public interface IResultFactory
    {
        IResult Create(IServiceContext context, object returnedObj);
    }
}
