using System.Xml.Serialization;
using RestFoundation;
using RestFoundation.Results;
using RestFoundation.Runtime;
using RestFoundation.ServiceProxy;

namespace RestTest.SimpleServices
{
    [ServiceContract]
    public class HelloService
    {
        [Url("", HttpMethod.Get)]
        [ProxyResourceExample(ResponseBuilderType = typeof(HelloWorldResponseBuilder))]
        public ContentResult Get()
        {
            return Result.Content("Hello world!", true, "text/plain");
        }
    }

    public class HelloWorldResponseBuilder : IResourceExampleBuilder
    {
        public object BuildInstance()
        {
            return "Hello world!";
        }
 
        public XmlSchemas BuildSchemas()
        {
            return XmlSchemaGenerator.Generate<string>();
        }
    }
}