using System.Security.Principal;

namespace RestFoundation
{
    public interface IServiceContext
    {
        IHttpRequest Request { get; }
        IHttpResponse Response { get; }

        IPrincipal User { get; set; }
        bool IsAuthenticated { get; }

        dynamic ItemBag { get; }

        string MapPath(string filePath);
    }
}
