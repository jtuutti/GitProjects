using System.Security.Principal;

namespace RestFoundation
{
    public interface IServiceContext
    {
        IPrincipal User { get; set; }
        bool IsAuthenticated { get; }

        dynamic ItemBag { get; }
    }
}
