using System;
using System.Security.Principal;
using System.Web;
using RestFoundation.Collections.Specialized;

namespace RestFoundation.Runtime
{
    public class ServiceContext : IServiceContext
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

        private static HttpContext Context
        {
            get
            {
                HttpContext context = HttpContext.Current;

                if (context == null)
                {
                    throw new InvalidOperationException("No HTTP context was found");
                }

                return context;
            }
        }

        public virtual IHttpRequest Request
        {
            get
            {
                return m_request;
            }
        }

        public virtual IHttpResponse Response
        {
            get
            {
                return m_response;
            }
        }

        public virtual IPrincipal User
        {
            get
            {
                return Context.User;
            }
            set
            {
                Context.User = value;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return Context.User != null && Context.User.Identity.IsAuthenticated;
            }
        }

        public virtual dynamic ItemBag
        {
            get
            {
                return new DynamicDictionary(Context.Items);
            }
        }
    }
}
