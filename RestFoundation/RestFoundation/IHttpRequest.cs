﻿using System.IO;
using System.Net;
using RestFoundation.Collections;
using RestFoundation.Runtime;

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

        NetworkCredential Credentials { get; }

        dynamic QueryBag { get; }

        IObjectValueCollection RouteValues { get; }
        IHeaderCollection Headers { get; }
        IStringValueCollection QueryString { get; }
        IStringValueCollection ServerVariables { get; }
        ICookieValueCollection Cookies { get; }
    }
}
