namespace MvcAlt
{
    public interface IBinder
    {
        string[] Bind(object resource, IHttpRequest request);
    }
}
