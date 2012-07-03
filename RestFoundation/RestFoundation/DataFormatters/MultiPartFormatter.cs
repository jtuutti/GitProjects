using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Web;
using RestFoundation.Runtime;

namespace RestFoundation.DataFormatters
{
    public class MultiPartFormatter : IDataFormatter
    {
        public object FormatRequest(IServiceContext context, Type objectType)
        {
            if (objectType != typeof(IEnumerable<IUploadedFile>) && objectType != typeof(ICollection<IUploadedFile>))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType, "Object type must be IEnumerable<IUploadedFile> or ICollection<IUploadedFile> for the content type");
            }

            var fileList = new List<IUploadedFile>();
            HttpContextBase httpContext = context.GetHttpContext();

            foreach (string fileName in httpContext.Request.Files.AllKeys)
            {
                fileList.Add(new UploadedFile(httpContext.Request.Files.Get(fileName)));
            }

            return new ReadOnlyCollection<IUploadedFile>(fileList);
        }

        public IResult FormatResponse(IServiceContext context, object obj)
        {
            throw new HttpResponseException(HttpStatusCode.NotAcceptable, "No supported content type was provided in the Accept or the Content-Type header");
        }
    }
}
