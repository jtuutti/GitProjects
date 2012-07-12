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
    public class XmlResult : IResult
    {
        public XmlResult()
        {
            ContentType = "application/xml";
        }

        public object Content { get; set; }
        public string ContentType { get; set; }

        public virtual void Execute(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            context.Response.Output.Clear();
            context.Response.SetHeader(context.Response.Headers.ContentType, ContentType);
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
