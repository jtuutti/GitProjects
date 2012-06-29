using System;
using System.IO;
using System.Xml.Serialization;
using RestFoundation.Results;
using RestFoundation.Runtime;

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

        public IResult FormatResponse(IHttpRequest request, IHttpResponse response, object obj)
        {
            var result = Rest.Active.CreateObject<XmlResult>();
            result.Content = obj;
            result.ContentType = request.GetPreferredAcceptType();

            return result;
        }
    }
}
