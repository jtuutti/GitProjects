using System;
using System.Runtime.CompilerServices;
using System.Xml;
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

        public void Execute(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (Content == null)
            {
                return;
            }

            context.Response.Output.Clear();
            context.Response.SetHeader("Content-Type", ContentType);
            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);

            OutputCompressionManager.FilterResponse(context);

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
            var xmlDocument = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(obj), "Object");

            var xmlWriter = new XmlTextWriter(response.Output.Writer)
            {
                Formatting = Formatting.None,
            };

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteRaw(xmlDocument.OuterXml);
            xmlWriter.Flush();
        }
    }
}
