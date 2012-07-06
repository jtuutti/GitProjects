using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using RestFoundation.Results;

namespace RestFoundation.DataFormatters
{
    public class BsonFormatter : IDataFormatter
    {
        public virtual object FormatRequest(IServiceContext context, Type objectType)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (objectType == null) throw new ArgumentNullException("objectType");

            if (context.Request.Body.CanSeek)
            {
                context.Request.Body.Seek(0, SeekOrigin.Begin);
            }

            using (var reader = new BsonReader(context.Request.Body))
            {
                var serializer = new JsonSerializer();

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

            var result = new BsonResult
                         {
                             Content = obj
                         };

            return result;
        }
    }
}
