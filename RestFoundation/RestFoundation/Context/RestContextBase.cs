// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Web;

namespace RestFoundation.Context
{
    /// <summary>
    /// Defines a base service context class.
    /// This class cannot be instantiated.
    /// </summary>
    public abstract class RestContextBase
    {
        /// <summary>
        /// Gets the underlying <see cref="HttpContextBase"/> instance.
        /// </summary>
        protected virtual HttpContextBase Context
        {
            get
            {
                HttpContext context = HttpContext.Current;

                if (context == null)
                {
                    throw new InvalidOperationException(Resources.Global.MissingHttpContext);
                }

                return new HttpContextWrapper(context);
            }
        }
    }
}
