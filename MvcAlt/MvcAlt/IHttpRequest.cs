using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace MvcAlt
{
    public interface IHttpRequest
    {
        Uri Url { get; }
        string RelativeUrl { get; }
        HttpVerb Verb { get; }

        Stream Body { get; }
        RouteValueDictionary RouteValues { get; }
        NameValueCollection Headers { get; }
        NameValueCollection Query { get; }
        NameValueCollection Form { get; }
        NameValueCollection ServerVariables { get; }
        HttpCookieCollection Cookies { get; }

        NameValueCollection Values { get; }

        string BodyAsString();
        string BodyAsString(Encoding encoding);

        string ToFullUrl(string url);
    }
}
