using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using RestFoundation.Collections.Specialized;

namespace RestFoundation.Context
{
    public class ServiceContext : ContextBase, IServiceContext
    {
        private readonly IHttpRequest m_request;
        private readonly IHttpResponse m_response;

        public ServiceContext(IHttpRequest request, IHttpResponse response)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (response == null) throw new ArgumentNullException("response");

            m_request = request;
            m_response = response;
        }

        public IHttpRequest Request
        {
            get
            {
                return m_request;
            }
        }

        public IHttpResponse Response
        {
            get
            {
                return m_response;
            }
        }

        public TimeSpan ServiceTimeout
        {
            get
            {
                return TimeSpan.FromSeconds(Context.Server.ScriptTimeout);
            }
            set
            {
                Context.Server.ScriptTimeout = Convert.ToInt32(value.TotalSeconds);
            }
        }

        public IPrincipal User
        {
            get
            {
                return Context.User;
            }
            set
            {
                Context.User = value;
                Thread.CurrentPrincipal = value;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return Context.User != null && Context.User.Identity.IsAuthenticated;
            }
        }

        public dynamic ItemBag
        {
            get
            {
                return new DynamicDictionary(Context.Items);
            }
        }

        public string MapPath(string filePath)
        {
            return Context.Server.MapPath(filePath);
        }

        public HttpContextBase GetHttpContext()
        {
            return Context;
        }
    }
}
