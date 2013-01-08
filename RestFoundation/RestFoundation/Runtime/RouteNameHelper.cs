// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RestFoundation.Runtime
{
    internal static class RouteNameHelper
    {
        public static string GetRouteName(string serviceUrl, MethodInfo method)
        {
            if (String.IsNullOrWhiteSpace(serviceUrl))
            {
                throw new ArgumentNullException("serviceUrl");
            }

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (method.DeclaringType == null)
            {
                throw new ArgumentOutOfRangeException("method");
            }

            string routeName = String.Format(CultureInfo.InvariantCulture,
                                          "{0}::{1}-{2}-{3}-{4}",
                                          serviceUrl.ToUpperInvariant(),
                                          method.DeclaringType.FullName + "," + method.DeclaringType.Assembly.GetName().FullName,
                                          method.ReturnType.FullName + "," + method.ReturnType.Assembly.GetName().FullName,
                                          method.Name,
                                          String.Join("-", method.GetParameters()
                                                                 .Select(x => String.Format(CultureInfo.InvariantCulture,
                                                                                            "{0}:{1},{2}",
                                                                                            x.Name,
                                                                                            x.ParameterType.FullName,
                                                                                            x.ParameterType.Assembly.GetName().FullName))
                                                                 .ToArray())).TrimEnd('-').Replace(" ", String.Empty);

            return ToBase64(routeName);
        }

        private static string ToBase64(string name)
        {
            byte[] nameBytes = Encoding.UTF8.GetBytes(name);

            return Convert.ToBase64String(nameBytes);
        }
    }
}
