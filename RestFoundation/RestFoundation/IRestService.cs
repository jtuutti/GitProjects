using System.Collections.Generic;

namespace RestFoundation
{
    public interface IRestService
    {
        IEnumerable<IServiceBehavior> Behaviors { get; }
    }
}
