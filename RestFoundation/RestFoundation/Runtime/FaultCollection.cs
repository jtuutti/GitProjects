using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents a fault collection.
    /// </summary>
    [XmlRoot("Faults")]
    public class FaultCollection
    {
        /// <summary>
        /// Gets or sets an array of general faults.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
                         Justification = "This class is designed for data serialization")]
        public Fault[] General { get; set; }

        /// <summary>
        /// Gets or sets an array of resource faults.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
                         Justification = "This class is designed for data serialization")]
        public Fault[] Resource { get; set; }
    }
}
