using System;
using System.Security.Principal;
using System.Web;
using RestFoundation.Collections.Specialized;

namespace RestFoundation.Runtime
{
    public class ServiceContext : IServiceContext
    {
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
    }
}
