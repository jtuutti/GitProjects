﻿using System;
using System.Web;

namespace RestFoundation.Context
{
    /// <summary>
    /// Defines a base service context class.
    /// </summary>
    public abstract class ContextBase
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
                    throw new InvalidOperationException("No HTTP context was found");
                }

                return new HttpContextWrapper(context);
            }
        }
    }
}
