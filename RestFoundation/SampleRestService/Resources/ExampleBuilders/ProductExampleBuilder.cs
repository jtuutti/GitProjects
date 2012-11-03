﻿using System.Linq;
using System.Xml.Serialization;
using RestFoundation.Runtime;
using RestFoundation.ServiceProxy;
using SampleRestService.DataAccess;

namespace SampleRestService.Resources.ExampleBuilders
{
    public sealed class ProductExampleBuilder : IResourceExampleBuilder
    {
        public object BuildInstance()
        {
            return new ProductRepository().GetAll().FirstOrDefault();
        }

        public XmlSchemas BuildSchemas()
        {
            return XmlSchemaGenerator.Generate<Product>();
        }
    }
}
