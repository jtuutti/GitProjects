// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;
using RestFoundation.Runtime;
using Formatting = System.Xml.Formatting;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents an XML result.
    /// </summary>
    public class XmlResult : IResult
    {
        /// <summary>
        /// Gets or sets the object to serialize to XML.
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        /// Gets or sets the content type. The "application/xml" content type is used by default.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Executes the result against the provided service context.
        /// </summary>
        /// <param name="context">The service context.</param>
        public virtual void Execute(IServiceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Response.Output.Clear();
            context.Response.SetHeader(context.Response.Headers.ContentType, ContentType ?? "application/xml");
            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);

            OutputCompressionManager.FilterResponse(context);

            if (Content == null)
            {
                SerializeNullObject(context.Response);
                return;
            }

            if (Attribute.GetCustomAttribute(Content.GetType(), typeof(CompilerGeneratedAttribute), false) != null)
            {
                SerializeAnonymousType(context.Response, Content);
                return;
            }

            XmlSerializer serializer = XmlSerializerRegistry.Get(Content.GetType());

            var xmlWriter = new XmlTextWriter(context.Response.Output.Writer)
            {
                Formatting = Formatting.None
            };

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            serializer.Serialize(xmlWriter, Content, namespaces);
        }

        private static void SerializeAnonymousType(IHttpResponse response, object obj)
        {
            var xmlDocument = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(obj), "complexType");

            var xmlWriter = new XmlTextWriter(response.Output.Writer)
            {
                Formatting = Formatting.None,
            };

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteRaw(xmlDocument.OuterXml);
            xmlWriter.Flush();
        }

        private static void SerializeNullObject(IHttpResponse response)
        {
            var xmlWriter = new XmlTextWriter(response.Output.Writer)
            {
                Formatting = Formatting.None,
            };

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("anyType");
            xmlWriter.WriteAttributeString("xmlns", "xsi", "http://www.w3.org/2000/xmlns/", XmlSchema.InstanceNamespace);
            xmlWriter.WriteAttributeString("xsi", "nil", XmlSchema.InstanceNamespace, "true");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
        }
    }
}
