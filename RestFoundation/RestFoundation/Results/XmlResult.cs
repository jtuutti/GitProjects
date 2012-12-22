// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
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
        private Encoding m_encoding;

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

            if (String.IsNullOrEmpty(ContentType))
            {
                ContentType = "application/json";
            }

            m_encoding = context.Request.Headers.AcceptCharsetEncoding;

            context.Response.Output.Clear();
            context.Response.SetHeader(context.Response.HeaderNames.ContentType, ContentType);
            context.Response.SetCharsetEncoding(m_encoding);

            var xmlWriter = XmlWriter.Create(context.Response.Output.Writer, new XmlWriterSettings
            {
                Encoding = m_encoding,
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

            LogResponse(Content);
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

        private void SerializeAnonymousType(XmlWriter xmlWriter, object obj)
        {
            XmlDocument xmlDocument = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(obj, Rest.Configuration.Options.JsonSettings.ToJsonSerializerSettings()), "complexType");
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteRaw(xmlDocument.OuterXml);
            xmlWriter.Flush();

            LogResponse(xmlDocument);
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

                LogResponse(enumerableContent);
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
        }

        private void LogResponse(object content)
        {
            if (content == null || !LogUtility.CanLog)
            {
                return;
            }

            string serializedContent = SerializeFormatted(content);
            LogUtility.LogResponseBody(serializedContent, ContentType);
        }

        private void LogResponse(XmlDocument document)
        {
            if (document == null || !LogUtility.CanLog)
            {
                return;
            }

            string serializedContent = ResourceOutputFormatter.FormatXml(document.OuterXml);
            LogUtility.LogResponseBody(serializedContent, ContentType);
        }

        private string SerializeFormatted(object content)
        {
            var serializer = XmlSerializerRegistry.Get(content.GetType());

            using (var stream = new MemoryStream())
            {
                var xmlWriter = new XmlTextWriter(stream, m_encoding)
                                {
                                    Formatting = Formatting.Indented
                                };

                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(String.Empty, String.Empty);

                serializer.Serialize(xmlWriter, content, namespaces);
                xmlWriter.Flush();

                stream.Seek(0, SeekOrigin.Begin);

                var xmlReader = new StreamReader(stream, m_encoding);
                return xmlReader.ReadToEnd();
            }
        }
    }
}
