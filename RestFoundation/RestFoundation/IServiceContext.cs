using System;
using System.Security.Principal;
using System.Web;

namespace RestFoundation
{
    public interface IServiceContext
    {
        IHttpRequest Request { get; }
        IHttpResponse Response { get; }

        TimeSpan ServiceTimeout { get; set; }
        IPrincipal User { get; set; }
        bool IsAuthenticated { get; }

        dynamic ItemBag { get; }

        string MapPath(string filePath);

        HttpContextBase GetHttpContext();
    }
}
