// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
namespace RestFoundation.Behaviors
{
    internal sealed class CacheValidationHandlerData
    {
        public CacheValidationHandlerData(IServiceContext serviceContext, MethodAuthorizingContext behaviorContext)
        {
            ServiceContext = serviceContext;
            BehaviorContext = behaviorContext;
        }

        public IServiceContext ServiceContext { get; private set; }
        public MethodAuthorizingContext BehaviorContext { get; private set; }
    }
}
