namespace RestFoundation
{
    public interface IResultFactory
    {
        IResult Create(object returnedObj);
    }
}
