using System.Net;

namespace RestFoundation
{
    public interface ICredentialResolver
    {
        NetworkCredential GetCredentials(IServiceContext context, IHttpRequest request);
    }
}
