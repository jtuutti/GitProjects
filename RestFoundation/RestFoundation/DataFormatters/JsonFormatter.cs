using System;
using System.IO;
using Newtonsoft.Json;
using RestFoundation.Results;
using RestFoundation.Runtime;

namespace RestFoundation.DataFormatters
{
    public class JsonFormatter : IDataFormatter
    {
        public virtual object FormatRequest(IServiceContext context, Type objectType)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (objectType == null) throw new ArgumentNullException("objectType");

            if (context.Request.Body.CanSeek)
            {
                context.Request.Body.Seek(0, SeekOrigin.Begin);
            }

            using (var streamReader = new StreamReader(context.Request.Body, context.Request.Headers.ContentCharsetEncoding))
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

        public virtual IResult FormatResponse(IServiceContext context, object obj)
        {
            if (context == null) throw new ArgumentNullException("context");

            var result = new JsonResult
                         {
                             Content = obj,
                             ContentType = context.Request.GetPreferredAcceptType()
                         };

            return result;
        }
    }
}
