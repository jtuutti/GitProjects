using System.Xml.Serialization;
using RestFoundation.Runtime;
using RestFoundation.ServiceProxy;

namespace SampleRestService.Resources.ExampleBuilders
{
    public sealed class NewProductExampleBuilder : IResourceExampleBuilder
    {
        public object BuildInstance()
        {
            return new Product
            {
                Name = "Bananas",
                Price = 4.59m,
                InStock = true
            };
        }

        public XmlSchemas BuildSchemas()
        {
            return XmlSchemaGenerator.Generate<Product>();
        }
    }
}
