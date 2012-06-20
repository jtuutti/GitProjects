using System.Xml.Serialization;

namespace RestTestContracts.Resources
{
    public class TouchMap
    {
        [XmlArray("QueryMaps"), XmlArrayItem("QueryMap")]
        public QueryMap[] QueryMaps { get; set; }

        [XmlArray("ServiceItems"), XmlArrayItem("ServiceItem")]
        public ServiceItem[] ServiceItems { get; set; }

        [XmlArray("Actions"), XmlArrayItem("Action")]
        public Action[] Actions { get; set; }
    }
}
