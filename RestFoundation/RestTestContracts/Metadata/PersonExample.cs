using System.Xml.Serialization;
using RestFoundation.Runtime;
using RestFoundation.ServiceProxy;
using RestTestContracts.Resources;

namespace RestTestContracts.Metadata
{
    public class PersonExample : IResourceExample
    {
        public object Create()
        {
            return new Person
            {
                Name = "Joe Bloe",
                Age = 41,
                Values = new[] { "01/21/1951" }
            };
        }

        public XmlSchemas GetSchemas()
        {
            return XmlSchemaGenerator.Generate<Person>();
        }
    }
}
