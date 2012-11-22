using System.Xml.Serialization;

namespace RestFoundation.ServiceProxy
{
    public sealed class ResourceExampleMetadata
    {
        public object Instance { get; set; }
        public XmlSchemas XmlSchemas { get; set; }
    }
}
