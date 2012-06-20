using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml.Serialization;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    public class XmlResult : IResult
    {
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

            Response.Clear();
            Response.SetHeader("Content-Type", "application/xml");
            Response.SetCharsetEncoding(Request.Headers.AcceptCharsetEncoding);

            EncodingManager.FilterResponse(Request, Response);

            Type contentType = Content.GetType();

            XmlSerializer serializer = contentType.IsArray ? GetSerializerForArray(contentType) : new XmlSerializer(contentType, ExtraTypes);

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            serializer.Serialize(Response.OutputWriter, Content, namespaces);
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
