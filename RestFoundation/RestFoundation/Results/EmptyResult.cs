using System;

namespace RestFoundation.Results
{
    public sealed class EmptyResult : IResult
    {
        public void Execute(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
        }
    }
}
