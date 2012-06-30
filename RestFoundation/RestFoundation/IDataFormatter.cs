using System;

namespace RestFoundation
{
    public interface IDataFormatter
    {
        object FormatRequest(IServiceContext context, Type objectType);
        IResult FormatResponse(IServiceContext context, object obj);
    }
}
