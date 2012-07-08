using System.Web;

namespace RestFoundation
{
    public interface IBrowserDetector
    {
        bool IsBrowserRequest(HttpRequestBase request);
    }
}
