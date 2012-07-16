using System.Xml.Serialization;
using RestFoundation.Runtime;
using RestFoundation.ServiceProxy;
using RestTestContracts.Resources;

namespace RestTestContracts.Metadata
{
    public class PersonArrayExampleBuilder : IResourceExampleBuilder
    {
        public object BuildInstance()
        {
            return new[]
            {
                new Person
                {
                    Name = "Joe Bloe",
                    Age = 41,
                    Values = new[] { "01/21/1971" }
                },
                new Person
                {
                    Name = "Mike Star",
                    Age = 21,
                    Values = new[] { "10/02/1991" }
                },
                new Person
                {
                    Name = "Beth Sue",
                    Age = 33
                }
            };
        }

        public XmlSchemas BuildSchemas()
        {
            return XmlSchemaGenerator.Generate<Person[]>();
        }
    }
}
