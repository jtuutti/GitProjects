namespace RestFoundation
{
    public interface IDataBinder
    {
        object Bind(IServiceContext context, string name);
    }
}
