using System.Net;

namespace RestFoundation
{
    public interface ICredentialResolver
    {
        NetworkCredential GetCredentials(IHttpRequest request);
    }
}
