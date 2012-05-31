namespace MvcAlt
{
    public interface IActionMethodInvoker
    {
        object Invoke(IHttpRequest request);
    }
}
