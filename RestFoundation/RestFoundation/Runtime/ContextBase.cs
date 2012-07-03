using System;
using System.Web;

namespace RestFoundation.Runtime
{
    public abstract class ContextBase
    {       
        protected virtual HttpContextBase Context
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
    }
}
