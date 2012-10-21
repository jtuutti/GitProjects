namespace RestFoundation.Tests.Implementation.Services
{
    [ServiceContract]
    public sealed class TestSelfContainedService
    {
        [Url("ok", HttpMethod.Get)]
        public string GetOK()
        {
            return "OK";
        }

        [Url("fail", HttpMethod.Get)]
        public string GetFail()
        {
            return "Fail";
        }
    }
}
