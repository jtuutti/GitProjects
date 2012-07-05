using RestFoundation;
using RestFoundation.ServiceProxy;

namespace RestTestContracts
{
    [ProxyAdditionalHeader("X-SpeechCycle-SmartCare-CustomerID", "AlphaMedia")]
    [ProxyAdditionalHeader("X-SpeechCycle-SmartCare-ApplicationID", "Mobile")]
    [ProxyAdditionalHeader("X-SpeechCycle-SmartCare-SessionID", "706035D4-3FD0-4CFD-AD40-0A951DA09838")]
    [ProxyAdditionalHeader("X-SpeechCycle-SmartCare-Environment", "Development")]
    [ProxyAdditionalHeader("X-SpeechCycle-SmartCare-Platform", "All")]
    [ProxyHttpsOnly(Port = 8443)]
    public interface ITouchMapService
    {
        [Url(Url.Root)]
        object Get();
    }
}
