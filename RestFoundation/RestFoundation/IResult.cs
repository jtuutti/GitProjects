namespace RestFoundation
{
    public interface IResult
    {
        IHttpRequest Request { get; }
        IHttpResponse Response { get; }

        void Execute();
    }
}
