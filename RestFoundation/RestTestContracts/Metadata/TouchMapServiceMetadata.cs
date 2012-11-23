using System.Collections.Generic;
using RestFoundation.ServiceProxy;

namespace RestTestContracts.Metadata
{
    public class TouchMapServiceMetadata : ProxyMetadata<ITouchMapService>
    {
        public override void Initialize()
        {
            SetHeaders(GetServiceHeaders());
            SetHttps(8443);

            ForMethod(x => x.Get()).SetDescription("Get a touchmap");
        }

        public static IList<ProxyHeader> GetServiceHeaders()
        {
            return new[]
            {
                new ProxyHeader("X-SpeechCycle-SmartCare-CustomerID", "AlphaMedia"),
                new ProxyHeader("X-SpeechCycle-SmartCare-ApplicationID", "Mobile"),
                new ProxyHeader("X-SpeechCycle-SmartCare-SessionID", "706035D4-3FD0-4CFD-AD40-0A951DA09838"),
                new ProxyHeader("X-SpeechCycle-SmartCare-Environment", "Development"),
                new ProxyHeader("X-SpeechCycle-SmartCare-Platform", "All")
            };
        }
    }
}
