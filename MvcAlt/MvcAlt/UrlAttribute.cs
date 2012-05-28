using System;

namespace MvcAlt
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class UrlAttribute : Attribute
    {
        public UrlAttribute(string url, params HttpVerb[] verbs)
        {
            if (url == null) throw new ArgumentNullException("url");
            if (url.Trim().Length == 0) throw new ArgumentException("Url template cannot be empty", "url");

            Url = url;
            Verbs = verbs ?? new HttpVerb[0];
        }

        public int Priority
        {
            get;
            set;
        }

        public string Url
        {
            get;
            private set;
        }

        public HttpVerb[] Verbs
        {
            get;
            set;
        }
    }
}
