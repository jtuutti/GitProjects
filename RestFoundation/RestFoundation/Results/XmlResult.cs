﻿// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
                ContentType = "application/xml";
            }
            else if (context.Request.Headers.AcceptVersion > 0 && ContentType.IndexOf("version=", StringComparison.OrdinalIgnoreCase) < 0)
            {
                ContentType += String.Format(CultureInfo.InvariantCulture, "; version={0}", context.Request.Headers.AcceptVersion);
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

            Type contentObjectType = Content.GetType();

            if (Attribute.IsDefined(contentObjectType, typeof(CompilerGeneratedAttribute), false))
            {
                SerializeAsAnonymousType(xmlWriter, Content);
                return;
            }

            if (IsGenericAnonymousCollectionType(contentObjectType)) 
            {
                SerializeAsAnonymousCollection(xmlWriter, Content);
                return;
            }

            if (ReturnedType == null && Content != null)
            {
                ReturnedType = contentObjectType;
            }

            if (ReturnedType != null && ReturnedType.IsGenericType && SerializeAsSpecializedCollection(context, xmlWriter))
            {
                return;
            }

            OutputCompressionManager.FilterResponse(context);

            XmlSerializer serializer = XmlSerializerRegistry.Get(Content.GetType());

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            serializer.Serialize(xmlWriter, Content, namespaces);

            LogResponse(Content);
        }

        private static bool IsGenericAnonymousCollectionType(Type objectType)
        {
            if (objectType.IsArray && Attribute.IsDefined(objectType.GetElementType(), typeof(CompilerGeneratedAttribute), false))
            {
                return true;
            }

            if (!objectType.IsGenericType)
            {
                return false;
            }

            Type genericType = objectType.GetGenericTypeDefinition();
            Type[] typeArguments = objectType.GetGenericArguments();

            if (typeArguments.Length != 1)
            {
                return false;
            }

            if (genericType == typeof(IQueryable<>) || genericType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryable<>)))
            {
                return false;
            }

            if (genericType != typeof(IEnumerable<>) && genericType.GetInterfaces().All(i => !i.IsGenericType || i.GetGenericTypeDefinition() != typeof(IEnumerable<>)))
            {
                return false;
            }

            return typeArguments[0] == typeof(object) || Attribute.IsDefined(typeArguments[0], typeof(CompilerGeneratedAttribute), false);
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

        private void SerializeAsAnonymousType(XmlWriter xmlWriter, object obj)
        {
            XmlDocument xmlDocument = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(obj, Rest.Configuration.Options.JsonSettings.ToJsonSerializerSettings()), "complexType");
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteRaw(xmlDocument.OuterXml);
            xmlWriter.Flush();

            LogResponse(xmlDocument);
        }

        private void SerializeAsAnonymousCollection(XmlWriter xmlWriter, object obj)
        {
            var wrapperObject = new
            {
                complexType = obj
            };

            XmlDocument xmlDocument = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(wrapperObject, Rest.Configuration.Options.JsonSettings.ToJsonSerializerSettings()), "complexTypes");
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteRaw(xmlDocument.OuterXml);
            xmlWriter.Flush();

            LogResponse(xmlDocument);
        }

        private void SerializeAsChunkedSequence(IServiceContext context, XmlWriter xmlWriter)
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

        private bool SerializeAsSpecializedCollection(IServiceContext context, XmlWriter xmlWriter)
        {
            if (ReturnedType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                SerializeAsChunkedSequence(context, xmlWriter);
                return true;
            }

            if (ReturnedType.GetGenericTypeDefinition() == typeof(IQueryable<>) ||
                ReturnedType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryable<>)))
            {
                Content = ODataHelper.PerformOdataOperations(Content, context.Request);
            }

            return false;
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
