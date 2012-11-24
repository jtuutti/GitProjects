using System.Xml.Serialization;

namespace RestFoundation.ServiceProxy.OperationMetadata
{
    /// <summary>
    /// Represents a resource example metadata.
    /// </summary>
    public sealed class ResourceExampleMetadata
    {
        /// <summary>
        /// Gets or sets a resource example instance.
        /// </summary>
        public object Instance { get; set; }

        /// <summary>
        /// Gets or sets XML schemas.
        /// </summary>
        public XmlSchemas XmlSchemas { get; set; }
    }
}
