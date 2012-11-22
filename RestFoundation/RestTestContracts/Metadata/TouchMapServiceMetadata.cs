using RestFoundation.ServiceProxy;

namespace RestTestContracts.Metadata
{
    public class TouchMapServiceMetadata : ProxyMetadata<ITouchMapService>
    {
        public override void Initialize()
        {
            SetHeader("X-SpeechCycle-SmartCare-CustomerID", "AlphaMedia");
            SetHeader("X-SpeechCycle-SmartCare-ApplicationID", "Mobile");
            SetHeader("X-SpeechCycle-SmartCare-SessionID", "706035D4-3FD0-4CFD-AD40-0A951DA09838");
            SetHeader("X-SpeechCycle-SmartCare-Environment", "Development");
            SetHeader("X-SpeechCycle-SmartCare-Platform", "All");

            SetHttps(8443);
        }
    }
}
