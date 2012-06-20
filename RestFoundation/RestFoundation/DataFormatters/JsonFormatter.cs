using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace RestFoundation.DataFormatters
{
    public class JsonFormatter : IDataFormatter
    {
        public object Format(Stream body, Encoding encoding, Type objectType)
        {
            if (body == null) throw new ArgumentNullException("body");
            if (encoding == null) throw new ArgumentNullException("encoding");
            if (objectType == null) throw new ArgumentNullException("objectType");

            using (var streamReader = new StreamReader(body, encoding))
            {
                body.Position = 0;

                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(streamReader);

                if (objectType == typeof(object))
                {
                    return serializer.Deserialize(reader);
                }

                return serializer.Deserialize(reader, objectType);
            }
        }
    }
}
