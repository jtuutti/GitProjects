using System.IO;
using RestFoundation.Collections;

namespace RestFoundation
{
    public interface IHttpRequest
    {
        bool IsAjax { get; }
        bool IsLocal { get; }
        bool IsSecure { get; }

        ServiceUri Url { get; }
        HttpMethod Method { get; }
        Stream Body { get; }

        dynamic QueryBag { get; }

        IObjectValueCollection RouteValues { get; }
        IHeaderCollection Headers { get; }
        IStringValueCollection QueryString { get; }
        IServerVariableCollection ServerVariables { get; }
        ICookieValueCollection Cookies { get; }
    }
}
