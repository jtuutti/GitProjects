using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RestFoundation.Runtime
{
    internal static class ActionMethodRegistry
    {
        private static readonly ConcurrentDictionary<Type, List<ActionMethodMetadata>> actionMethods = new ConcurrentDictionary<Type, List<ActionMethodMetadata>>();

        public static ConcurrentDictionary<Type, List<ActionMethodMetadata>> ActionMethods
        {
            get
            {
                return actionMethods;
            }
        }

        public static MethodInfo GetActionMethod(Type serviceContractType, string urlTemplate, HttpMethod httpMethod, out OutputCacheAttribute cache)
        {
            if (serviceContractType == null) throw new ArgumentNullException("serviceContractType");
            if (urlTemplate == null) throw new ArgumentNullException("urlTemplate");

            List<ActionMethodMetadata> serviceActionMethods;

            if (!actionMethods.TryGetValue(serviceContractType, out serviceActionMethods))
            {
                cache = null;
                return null;
            }

            foreach (var actionMethod in serviceActionMethods)
            {
                if (String.Equals(UrlTemplateStandardizer.Standardize(actionMethod.UrlInfo.UrlTemplate), UrlTemplateStandardizer.Standardize(urlTemplate)) &&
                    actionMethod.UrlInfo.HttpMethods.Contains(httpMethod))
                {
                    cache = actionMethod.OutputCache;
                    return actionMethod.MethodInfo;
                }
            }

            cache = null;
            return null;
        }
    }
}
