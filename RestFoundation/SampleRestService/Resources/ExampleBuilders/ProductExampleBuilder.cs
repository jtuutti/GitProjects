using System.Linq;
using System.Xml.Serialization;
using RestFoundation.Runtime;
using RestFoundation.ServiceProxy;
using SampleRestService.DataAccess;

namespace SampleRestService.Resources.ExampleBuilders
{
    public sealed class ProductExampleBuilder : IResourceExampleBuilder
    {
        /// <summary>
        /// Builds and returns an instance of the example resource class.
        /// </summary>
        /// <returns>
        /// An example resource object instance.
        /// </returns>
        public object BuildInstance()
        {
            return new ProductRepository().GetAll().FirstOrDefault();
        }

        /// <summary>
        /// Builds and returns XML schemas for the resource class.
        /// See <seealso cref="T:RestFoundation.Runtime.XmlSchemaGenerator"/> for an automatic XML schema generator.
        /// Return null from this method, if you do not need XML schemas to be displayed.
        /// </summary>
        /// <returns>
        /// An list of XML schemas for the response object.
        /// </returns>
        public XmlSchemas BuildSchemas()
        {
            return XmlSchemaGenerator.Generate<Product>();
        }
    }
}
