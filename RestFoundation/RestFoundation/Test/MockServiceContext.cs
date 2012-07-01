using System;
using System.Security.Principal;
using System.Threading;
using RestFoundation.Collections.Specialized;

namespace RestFoundation.Test
{
    public class MockServiceContext : IServiceContext
    {
        private readonly string m_relativeServiceUrl;

        public MockServiceContext(string relativeServiceUrl)
        {
            if (String.IsNullOrEmpty(relativeServiceUrl)) throw new ArgumentNullException("relativeServiceUrl");

            m_relativeServiceUrl = relativeServiceUrl.TrimStart('~').Trim('/');

            Request = new MockHttpRequest(m_relativeServiceUrl);
            Response = new MockHttpResponse();
            ItemBag = new DynamicDictionary();
        }

        public virtual IHttpRequest Request { get; set; }
        public virtual IHttpResponse Response { get; set; }
        public virtual dynamic ItemBag { get; set; }

        public virtual IPrincipal User
        {
            get
            {
                return Thread.CurrentPrincipal;
            }
            set
            {
                Thread.CurrentPrincipal = value;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return User != null && User.Identity.IsAuthenticated;
            }
        }

        public virtual string MapPath(string filePath)
        {
            if (filePath == null)
            {
                return null;
            }

            return filePath.ToLowerInvariant().Replace("http://localhost/" + m_relativeServiceUrl, Environment.CurrentDirectory).Replace("/", @"\").TrimStart('~', '\\');
        }
    }
}
