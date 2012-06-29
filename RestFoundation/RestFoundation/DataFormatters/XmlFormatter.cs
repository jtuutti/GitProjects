using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using RestFoundation.Runtime;
using Formatting = System.Xml.Formatting;

namespace RestFoundation.DataFormatters
{
    public class XmlFormatter : IDataFormatter
    {
        public object FormatRequest(IHttpRequest request, Type objectType)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (objectType == null) throw new ArgumentNullException("objectType");

            if (objectType == typeof(object))
            {
                using (var streamReader = new StreamReader(request.Body, request.Headers.ContentCharsetEncoding))
                {
                    return new DynamicXDocument(streamReader.ReadToEnd());
                }
            }

            if (request.Body.CanSeek)
            {
                request.Body.Seek(0, SeekOrigin.Begin);
            }

            var serializer = new XmlSerializer(objectType);
            return serializer.Deserialize(request.Body);
        }

        public void FormatResponse(IHttpRequest request, IHttpResponse response, object obj)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (response == null) throw new ArgumentNullException("response");

            if (obj == null)
            {
                return;
            }

            response.Output.Clear();
            response.SetHeader("Content-Type", request.GetPreferredAcceptType());
            response.SetCharsetEncoding(request.Headers.AcceptCharsetEncoding);

            OutputCompressionManager.FilterResponse(request, response);

            if (Attribute.GetCustomAttribute(obj.GetType(), typeof(CompilerGeneratedAttribute), false) != null)
            {
                SerializeAnonymousType(response, obj);
                return;
            }

            XmlSerializer serializer = XmlSerializerRegistry.Get(obj.GetType());

            var xmlWriter = new XmlTextWriter(response.Output.Writer)
            {
                Formatting = Formatting.None
            };

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            serializer.Serialize(xmlWriter, obj, namespaces);
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
