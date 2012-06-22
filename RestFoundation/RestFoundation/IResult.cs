namespace RestFoundation
{
    public interface IResult
    {
        IServiceContext Context { get; }
        IHttpRequest Request { get; }
        IHttpResponse Response { get; }

        void Execute();
    }
}
