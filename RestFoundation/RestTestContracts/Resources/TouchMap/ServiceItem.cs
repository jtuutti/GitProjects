using System.Xml.Serialization;

namespace RestTestContracts.Resources
{
    public class ServiceItem
    {
        public string Action { get; set; }
        public string Parent { get; set; }
        public bool MenuItemOnly { get; set; }
        public bool NoResultsItem { get; set; }
        public string Text { get; set; }
        public string ConfirmationTemplate { get; set; }
        public string TipText { get; set; }

        [XmlArray("SearchResultNames"), XmlArrayItem("SearchResultName")]
        public string[] SearchResultNames { get; set; }

        [XmlArray("QueryTerms"), XmlArrayItem("QueryTerm")]
        public string[] QueryTerms { get; set; }
    }
}
