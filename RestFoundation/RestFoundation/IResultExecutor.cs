using System;

namespace RestFoundation
{
    public interface IResultExecutor
    {
        void Execute(IServiceContext context, IResult result, Type methodReturnType);
    }
}