using System.Xml.Serialization;

namespace RestTestContracts.Resources
{
    public class Action
    {
        public string Name { get; set; }

        [XmlArray("Targets"), XmlArrayItem("Target")]
        public Target[] Targets { get; set; }

        [XmlArray("Variables"), XmlArrayItem("Variable")]
        public string[] Variables { get; set; }

        [XmlArray("Constants"), XmlArrayItem("Constant")]
        public Constant[] Constants { get; set; }
    }
}
