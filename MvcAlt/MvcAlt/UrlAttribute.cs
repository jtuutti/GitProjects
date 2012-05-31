using System;

namespace MvcAlt
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class UrlAttribute : Attribute
    {
        public UrlAttribute(string url, params HttpVerb[] verbs)
        {
            if (url == null) throw new ArgumentNullException("url");

            Url = url;
            Verbs = (verbs != null && verbs.Length > 0) ? verbs : new[] { HttpVerb.Get, HttpVerb.Head };
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
