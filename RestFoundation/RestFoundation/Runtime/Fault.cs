// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents a general or a resource fault.
    /// </summary>
    public class Fault : IXmlSerializable
    {
        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the fault message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return
        /// null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the
        /// <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced
        /// by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed
        /// by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader)
        {
            var navigator = new XPathDocument(reader).CreateNavigator();

            XPathNavigator propertyNameNode = navigator.SelectSingleNode("//Fault/PropertyName");

            if (propertyNameNode != null)
            {
                PropertyName = propertyNameNode.Value;
            }

            XPathNavigator messageNode = navigator.SelectSingleNode("//Fault/Message");

            if (messageNode != null)
            {
                Message = messageNode.Value;
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
                         Justification = "This is a serializer called method that will always provide the writer.")]
        public void WriteXml(XmlWriter writer)
        {
            if (!String.IsNullOrEmpty(PropertyName))
            {
                writer.WriteElementString("PropertyName", PropertyName);
            }

            writer.WriteStartElement("Message");
            writer.WriteCData(Message ?? String.Empty);
            writer.WriteEndElement();
        }
    }
}
