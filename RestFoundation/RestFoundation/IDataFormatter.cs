using System;

namespace RestFoundation
{
    public interface IDataFormatter
    {
        object FormatRequest(IHttpRequest request, Type objectType);
        IResult FormatResponse(IHttpRequest request, IHttpResponse response, object obj);
    }
}
