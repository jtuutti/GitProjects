namespace RestFoundation.Results
{
    public sealed class EmptyResult : IResult
    {
        public IServiceContext Context { get; set; }
        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }

        public void Execute()
        {
        }
    }
}
