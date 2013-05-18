// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
namespace RestFoundation.Configuration
{
    /// <summary>
    /// Contains settings used by XML formatters and results.
    /// </summary>
    public sealed class XmlFormatterSettings
    {
        /// <summary>
        /// Gets or sets the XML namespace
        /// </summary>
        public string Namespace { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether XML declaration should be omitted
        /// by the XML formatter.
        /// </summary>
        public bool OmitXmlDeclaration { get; set; }
    }
}
