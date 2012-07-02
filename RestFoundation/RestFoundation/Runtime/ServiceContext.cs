using System;
using System.Security.Principal;
using System.Web;
using RestFoundation.Collections.Specialized;

namespace RestFoundation.Runtime
{
    public sealed class ServiceContext : IServiceContext
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

        private static HttpContextBase Context
        {
            get
            {
                HttpContext context = HttpContext.Current;

                if (context == null)
                {
                    throw new InvalidOperationException("No HTTP context was found");
                }

                return new HttpContextWrapper(context);
            }
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

        public IPrincipal User
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
    }
}
