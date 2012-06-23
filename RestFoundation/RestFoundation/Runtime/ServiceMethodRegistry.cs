using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RestFoundation.Runtime
{
    internal static class ServiceMethodRegistry
    {
        private static readonly ConcurrentDictionary<ServiceMetadata, List<ServiceMethodMetadata>> serviceMethods = new ConcurrentDictionary<ServiceMetadata, List<ServiceMethodMetadata>>();

        public static ConcurrentDictionary<ServiceMetadata, List<ServiceMethodMetadata>> ServiceMethods
        {
            get
            {
                return serviceMethods;
            }
        }

        public static MethodInfo GetMethod(ServiceMetadata metadata, string urlTemplate, HttpMethod httpMethod, out ValidateAclAttribute acl, out OutputCacheAttribute cache)
        {
            List<ServiceMethodMetadata> methods;

            if (!serviceMethods.TryGetValue(metadata, out methods))
            {
                acl = null;
                cache = null;
                return null;
            }

            foreach (var method in methods)
            {
                if (String.Equals(UrlTemplateStandardizer.Standardize(method.UrlInfo.UrlTemplate), UrlTemplateStandardizer.Standardize(urlTemplate)) &&
                    method.UrlInfo.HttpMethods.Contains(httpMethod))
                {
                    acl = method.Acl;
                    cache = method.OutputCache;
                    return method.MethodInfo;
                }
            }

            acl = null;
            cache = null;
            return null;
        }
    }
}
