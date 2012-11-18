using System.Xml.Serialization;

namespace RestFoundation.ServiceProxy
{
    public sealed class ResourceBuilderMetadata
    {
        public object Instance { get; set; }
        public XmlSchemas XmlSchemas { get; set; }
    }
}
