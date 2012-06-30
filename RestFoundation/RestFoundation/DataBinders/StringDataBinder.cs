using System;

namespace RestFoundation.DataBinders
{
    public class StringDataBinder : IDataBinder
    {
        public object Bind(IServiceContext context, string name)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (String.IsNullOrEmpty(name))
            {
                return null;
            }

            return context.Request.QueryString.TryGet(name) ??
                   context.Request.RouteValues.TryGet(name);
        }
    }
}
