using System.Linq;
using System.Xml.Serialization;
using RestFoundation.Runtime;
using RestFoundation.ServiceProxy;
using SampleRestService.DataAccess;

namespace SampleRestService.Resources.ExampleBuilders
{
    public sealed class ProductArrayExampleBuilder : IResourceExampleBuilder
    {
        public object BuildInstance()
        {
            return new ProductRepository().GetAll().Take(3).ToList();
        }

        public XmlSchemas BuildSchemas()
        {
            return XmlSchemaGenerator.Generate<Product[]>();
        }
    }
}
