using RestFoundation;
using RestFoundation.Results;
using RestFoundation.ServiceProxy;

namespace RestTest.SimpleServices
{
    [ServiceContract]
    public class HelloService : ProxyMetadata<HelloService>
    {
        [Url("", HttpMethod.Get)]
        public ContentResult Get()
        {
            return Result.Content("Hello world!", true, "text/plain");
        }

        public override void Initialize()
        {
            ForMethod(x => x.Get()).SetDescription("Prints 'Hello world!'")
                                   .SetResponseResourceExample("Hello world!");
        }
    }
}
