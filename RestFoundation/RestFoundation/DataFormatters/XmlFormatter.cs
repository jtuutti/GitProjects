using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace RestFoundation.DataFormatters
{
    public class XmlFormatter : IDataFormatter
    {
        public object Format(Stream body, Encoding encoding, Type objectType)
        {
            if (body == null) throw new ArgumentNullException("body");
            if (encoding == null) throw new ArgumentNullException("encoding");
            if (objectType == null) throw new ArgumentNullException("objectType");

            if (objectType == typeof(object))
            {
                using (var streamReader = new StreamReader(body, encoding))
                {
                    return new DynamicXDocument(streamReader.ReadToEnd());
                }
            }

            var serializer = new XmlSerializer(objectType);
            return serializer.Deserialize(body);
        }
    }
}
