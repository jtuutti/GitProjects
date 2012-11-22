using RestFoundation.ServiceProxy;

namespace RestTestContracts.Metadata
{
    public class DynamicServiceMetadata : ProxyMetadata<IDynamicService>
    {
        public override void Initialize()
        {
            ForMethod(x => x.Post(null)).SetDescription("Makes use of dynamically typed resource capabilities");
        }
    }
}
