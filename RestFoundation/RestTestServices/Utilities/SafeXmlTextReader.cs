using System;
using System.IO;
using System.Xml;

namespace RestTestServices.Utilities
{
    /// <summary>
    /// Represents a reader that provides fast, non-cached, forward-only access to XML data.
    /// This version of the XML text reader is namespace safe.
    /// </summary>
    public class SafeXmlTextReader : XmlTextReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SafeXmlTextReader"/> class with the specified stream.
        /// </summary>
        /// <param name="input">The stream containing the XML data to read.</param>
        public SafeXmlTextReader(Stream input) : base(input)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeXmlTextReader"/> class with the specified stream.
        /// </summary>
        /// <param name="input">The stream containing the XML data to read.</param>
        /// <param name="nt">The <see cref="XmlNameTable"/> to use.</param>
        public SafeXmlTextReader(Stream input, XmlNameTable nt) : base(input, nt)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeXmlTextReader"/> class with the specified text reader.
        /// </summary>
        /// <param name="input">The <see cref="TextReader"/> containing the XML data to read.</param>
        public SafeXmlTextReader(TextReader input) : base(input)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeXmlTextReader"/> class with the specified text reader.
        /// </summary>
        /// <param name="input">The <see cref="TextReader"/> containing the XML data to read.</param>
        /// <param name="nt">The <see cref="XmlNameTable"/> to use.</param>
        public SafeXmlTextReader(TextReader input, XmlNameTable nt) : base(input, nt)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeXmlTextReader"/> class with the specified stream,
        /// <see cref="XmlNodeType"/>, and <see cref="XmlParserContext"/>.
        /// </summary>
        /// <param name="xmlFragment">The stream containing the XML fragment to parse.</param>
        /// <param name="fragType">
        /// The <see cref="XmlNodeType"/> of the XML fragment. This also determines what the fragment can contain.
        /// </param>
        /// <param name="context">
        /// The <see cref="XmlParserContext"/> in which the xmlFragment is to be parsed. This includes the
        /// <see cref="XmlNameTable"/> to use, encoding, namespace scope, the current xml:lang, and the xml:space
        /// scope.
        /// </param>
        /// <exception cref="XmlException">
        /// fragType is not an Element, Attribute, or Document <see cref="XmlNodeType"/>.
        /// </exception>
        public SafeXmlTextReader(Stream xmlFragment, XmlNodeType fragType, XmlParserContext context) : base(xmlFragment, fragType, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeXmlTextReader"/> class with the specified <see cref="string"/>,
        /// <see cref="XmlNodeType"/>, and <see cref="XmlParserContext"/>.
        /// </summary>
        /// <param name="xmlFragment">The <see cref="string"/> containing the XML fragment to parse.</param>
        /// <param name="fragType">
        /// The <see cref="XmlNodeType"/> of the XML fragment. This also determines what the fragment can contain.
        /// </param>
        /// <param name="context">
        /// The <see cref="XmlParserContext"/> in which the xmlFragment is to be parsed. This includes the
        /// <see cref="XmlNameTable"/> to use, encoding, namespace scope, the current xml:lang, and the xml:space
        /// scope.
        /// </param>
        /// <exception cref="XmlException">
        /// fragType is not an Element, Attribute, or Document <see cref="XmlNodeType"/>.
        /// </exception>
        public SafeXmlTextReader(string xmlFragment, XmlNodeType fragType, XmlParserContext context) : base(xmlFragment, fragType, context)
        {
        }

        /// <summary>
        /// Gets the namespace URI (as defined in the W3C Namespace specification) of the node on which the reader is positioned.
        /// </summary>
        /// <returns>
        /// The namespace URI of the current node; otherwise an empty string.
        /// </returns>
        public override string NamespaceURI
        {
            get
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Reads the contents of an element or a text node as a string.
        /// </summary>
        /// <returns>
        /// The contents of the element or text node. This can be an empty string if the reader is positioned on something
        /// other than an element or text node, or if there is no more text content to return in the current context.
        /// Note: The text node can be either an element or an attribute text node.
        /// </returns>
        /// <exception cref="T:System.Xml.XmlException">An error occurred while parsing the XML.</exception>
        /// <exception cref="T:System.InvalidOperationException">An invalid operation was attempted.</exception>
        public override string ReadString()
        {
            string value = base.ReadString();

            if (value == null)
            {
                return null;
            }

            return value.Trim(' ', '\n', '\r', '\t');
        }
    }
}
