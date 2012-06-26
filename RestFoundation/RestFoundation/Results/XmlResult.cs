using System;
using System.Collections.Concurrent;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    public class XmlResult : IResult
    {
        private static readonly ConcurrentDictionary<Type, XmlSerializer> serializers = new ConcurrentDictionary<Type, XmlSerializer>();

        public IServiceContext Context { get; set; }
        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }
        public object Content { get; set; }
        public Type[] ExtraTypes { get; set; }

        public void Execute()
        {
            if (Response == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No HTTP context found");
            }

            if (Content == null)
            {
                return;
            }

            Response.Output.Clear();
            Response.SetHeader("Content-Type", "application/xml");
            Response.SetCharsetEncoding(Request.Headers.AcceptCharsetEncoding);

            OutputCompressionManager.FilterResponse(Request, Response);

            XmlSerializer serializer = serializers.GetOrAdd(Content.GetType(), GetSerializerForType);

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            var xmlWriter = new XmlTextWriter(Response.Output.Writer)
            {
                Formatting = Formatting.None
            };

            serializer.Serialize(xmlWriter, Content, namespaces);
        }

        private XmlSerializer GetSerializerForType(Type contentType)
        {
            return contentType.IsArray ? GetSerializerForArray(contentType) : new XmlSerializer(contentType, ExtraTypes);
        }

        private XmlSerializer GetSerializerForArray(Type objectType)
        {
            XmlSerializer serializer;

            Type elementType = objectType.GetElementType();
            var xmlRootAttribute = elementType.GetCustomAttributes(typeof(XmlRootAttribute), true).OfType<XmlRootAttribute>().FirstOrDefault();

            string elementName = xmlRootAttribute != null && !String.IsNullOrEmpty(xmlRootAttribute.ElementName) ? xmlRootAttribute.ElementName : elementType.Name;

            var pluralization = PluralizationService.CreateService(CultureInfo.CurrentCulture);

            if (!pluralization.IsPlural(elementName))
            {
                elementName = pluralization.Pluralize(elementName);
            }

            if (xmlRootAttribute != null && !String.IsNullOrEmpty(xmlRootAttribute.Namespace))
            {
                serializer = new XmlSerializer(objectType, null, ExtraTypes, new XmlRootAttribute(elementName) { Namespace = xmlRootAttribute.Namespace }, null);
            }
            else
            {
                serializer = new XmlSerializer(objectType, null, ExtraTypes, new XmlRootAttribute(elementName), null);
            }

            return serializer;
        }
    }
}
