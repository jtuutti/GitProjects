﻿using System;
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
        public XmlResult()
        {
            ContentType = "application/xml";
        }

        public IServiceContext Context { get; set; }
        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }
        public object Content { get; set; }
        public string ContentType { get; set; }

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
            Response.SetHeader("Content-Type", ContentType);
            Response.SetCharsetEncoding(Request.Headers.AcceptCharsetEncoding);

            OutputCompressionManager.FilterResponse(Request, Response);

            if (Attribute.GetCustomAttribute(Content.GetType(), typeof(CompilerGeneratedAttribute), false) != null)
            {
                SerializeAnonymousType(Response, Content);
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
