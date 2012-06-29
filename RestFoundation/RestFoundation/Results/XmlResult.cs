using System;
using System.Net;
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
        public IServiceContext Context { get; set; }
        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }
        public object Content { get; set; }

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

            if (Attribute.GetCustomAttribute(Content.GetType(), typeof(CompilerGeneratedAttribute), false) != null)
            {
                var xmlDocument = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(Content), "Object");
                Response.Output.Write(xmlDocument.OuterXml);

                return;
            }

            XmlSerializer serializer = XmlSerializerRegistry.Get(Content.GetType());

            var xmlWriter = new XmlTextWriter(Response.Output.Writer)
            {
                Formatting = Formatting.None
            };

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            serializer.Serialize(xmlWriter, Content, namespaces);
        }
    }
}
