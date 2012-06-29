using System.Xml.Serialization;
using RestFoundation.Runtime;
using RestFoundation.ServiceProxy;
using RestTestContracts.Resources;

namespace RestTestContracts.Metadata
{
    public class PersonArrayExample : IResourceExample
    {
        public object Create()
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

        public XmlSchemas GetSchemas()
        {
            return XmlSchemaGenerator.Generate<Person[]>();
        }
    }
}
