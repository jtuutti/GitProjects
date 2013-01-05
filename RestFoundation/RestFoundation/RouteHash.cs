using System;
using System.Collections.Generic;
using System.Web.Routing;

namespace RestFoundation
{
    /// <summary>
    /// Represents a route value dictionary in a hash format.
    /// </summary>
    public class RouteHash : RouteValueDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteHash"/> class.
        /// </summary>
        /// <param name="values">A sequence of key-value pairs represented by lambda expressions.</param>
        public RouteHash(params Func<object, object>[] values)
        {
            if (values == null || values.Length == 0)
            {
                return;
            }

            foreach (Func<object, object> func in values)
            {
                string key = func.Method.GetParameters()[0].Name;
                this[key] = func(null);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteHash"/> class.
        /// </summary>
        /// <param name="valueDictionary">A dictionary of route values.</param>
        public RouteHash(IDictionary<string, object> valueDictionary) : base(valueDictionary)
        {
        }
    }
}
