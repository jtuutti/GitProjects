using System;
using System.IO;
using System.Xml.Serialization;
using RestFoundation.Context;
using RestFoundation.Results;

namespace RestFoundation.DataFormatters
{
    public class XmlFormatter : IDataFormatter
    {
        public virtual object FormatRequest(IServiceContext context, Type objectType)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (objectType == null) throw new ArgumentNullException("objectType");

            if (objectType == typeof(object))
            {
                using (var streamReader = new StreamReader(context.Request.Body, context.Request.Headers.ContentCharsetEncoding))
                {
                    return new DynamicXDocument(streamReader.ReadToEnd());
                }
            }

            if (context.Request.Body.CanSeek)
            {
                context.Request.Body.Seek(0, SeekOrigin.Begin);
            }

            XmlSerializer serializer;

            try
            {
                serializer = new XmlSerializer(objectType);
            }
            catch (NotSupportedException) // cannot find a serializer for the type
            {
                return null;
            }

            return serializer.Deserialize(context.Request.Body);
        }

        public virtual IResult FormatResponse(IServiceContext context, object obj)
        {
            if (context == null) throw new ArgumentNullException("context");

            return new XmlResult
            {
                Content = obj,
                ContentType = context.Request.GetPreferredAcceptType()
            };
        }
    }
}
