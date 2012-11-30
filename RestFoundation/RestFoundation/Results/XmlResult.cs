// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;
using RestFoundation.Runtime;

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

        internal Type ReturnedType { get; set; }

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

            var xmlWriter = XmlWriter.Create(context.Response.Output.Writer, new XmlWriterSettings
            {
                Encoding = context.Request.Headers.AcceptCharsetEncoding,
                Indent = false
            });

            if (Content == null)
            {
                SerializeNullObject(xmlWriter);
                return;
            }

            if (Attribute.IsDefined(Content.GetType(), typeof(CompilerGeneratedAttribute), false))
            {
                SerializeAnonymousType(xmlWriter, Content);
                return;
            }

            if (ReturnedType != null && ReturnedType.IsGenericType && ReturnedType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                SerializeChunkedSequence(context, xmlWriter);
                return;
            }

            OutputCompressionManager.FilterResponse(context);

            XmlSerializer serializer = XmlSerializerRegistry.Get(Content.GetType());

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            serializer.Serialize(xmlWriter, Content, namespaces);
        }

        private static void SerializeAnonymousType(XmlWriter xmlWriter, object obj)
        {
            XmlDocument xmlDocument = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(obj), "complexType");
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteRaw(xmlDocument.OuterXml);
            xmlWriter.Flush();
        }

        private static void SerializeNullObject(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("anyType");
            xmlWriter.WriteAttributeString("xmlns", "xsi", "http://www.w3.org/2000/xmlns/", XmlSchema.InstanceNamespace);
            xmlWriter.WriteAttributeString("xsi", "nil", XmlSchema.InstanceNamespace, "true");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
        }

        private void SerializeChunkedSequence(IServiceContext context, XmlWriter xmlWriter)
        {
            var enumerableContent = (IEnumerable) Content;

            string rootNamespace;
            string rootElementName = XmlRootElementInspector.GetRootElementName(Content.GetType().GetGenericArguments()[0], out rootNamespace);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement(rootElementName);
            xmlWriter.Flush();
            context.Response.Output.Flush();

            foreach (object enumeratedContent in enumerableContent)
            {
                if (enumeratedContent == null)
                {
                    continue;
                }

                XmlSerializer serializer = XmlSerializerRegistry.Get(enumeratedContent.GetType());

                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(String.Empty, String.Empty);

                serializer.Serialize(xmlWriter, enumeratedContent, namespaces);
                context.Response.Output.Flush();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
        }
    }
}
