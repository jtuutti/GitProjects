using System.Xml.Serialization;
using RestFoundation.Runtime;
using RestFoundation.ServiceProxy;
using RestTestContracts.Resources;

namespace RestTestContracts.Metadata
{
    public class PersonExampleBuilder : IResourceExampleBuilder
    {
        public object BuildInstance()
        {
            return new Person
            {
                Name = "Joe Bloe",
                Age = 41,
                Values = new[] { "01/21/1951" }
            };
        }

        public XmlSchemas BuildSchemas()
        {
            return XmlSchemaGenerator.Generate<Person>();
        }
    }
}
