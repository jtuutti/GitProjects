using System;
using System.IO;
using Newtonsoft.Json;
using RestFoundation.Results;
using RestFoundation.Runtime;

namespace RestFoundation.DataFormatters
{
    public class JsonFormatter : IDataFormatter
    {
        public object FormatRequest(IHttpRequest request, Type objectType)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (objectType == null) throw new ArgumentNullException("objectType");

            if (request.Body.CanSeek)
            {
                request.Body.Seek(0, SeekOrigin.Begin);
            }

            using (var streamReader = new StreamReader(request.Body, request.Headers.ContentCharsetEncoding))
            {
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(streamReader);

                if (objectType == typeof(object))
                {
                    return serializer.Deserialize(reader);
                }

                return serializer.Deserialize(reader, objectType);
            }
        }

        public IResult FormatResponse(IHttpRequest request, IHttpResponse response, object obj)
        {
            var result = Rest.Active.CreateObject<JsonResult>();
            result.Content = obj;
            result.ContentType = request.GetPreferredAcceptType();

            return result;
        }
    }
}
