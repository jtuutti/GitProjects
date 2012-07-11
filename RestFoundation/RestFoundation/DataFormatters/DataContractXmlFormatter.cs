using System;
using System.IO;
using System.Runtime.Serialization;
using RestFoundation.Context;
using RestFoundation.Results;

namespace RestFoundation.DataFormatters
{
    public class DataContractXmlFormatter : IDataFormatter
    {
        public virtual object FormatRequest(IServiceContext context, Type objectType)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (objectType == null) throw new ArgumentNullException("objectType");

            if (context.Request.Body.CanSeek)
            {
                context.Request.Body.Seek(0, SeekOrigin.Begin);
            }

            var serializer = new DataContractSerializer(objectType);

            return serializer.ReadObject(context.Request.Body);
        }

        public virtual IResult FormatResponse(IServiceContext context, object obj)
        {
            if (context == null) throw new ArgumentNullException("context");

            return new DataContractXmlResult
            {
                Content = obj,
                ContentType = context.Request.GetPreferredAcceptType()
            };
        }
    }
}
