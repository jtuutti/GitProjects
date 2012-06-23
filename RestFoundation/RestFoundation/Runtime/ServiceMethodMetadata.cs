using System;
using System.Reflection;

namespace RestFoundation.Runtime
{
    internal struct ServiceMethodMetadata : IEquatable<ServiceMethodMetadata>
    {
        private readonly Guid m_serviceMethodId;
        private readonly string m_serviceUrl;
        private readonly MethodInfo m_methodInfo;
        private readonly UrlAttribute m_urlInfo;
        private readonly ValidateAclAttribute m_acl;
        private readonly OutputCacheAttribute m_outputCache;

        public ServiceMethodMetadata(string serviceUrl, MethodInfo methodInfo, UrlAttribute urlInfo, ValidateAclAttribute acl, OutputCacheAttribute outputCache)
        {
            if (serviceUrl == null) throw new ArgumentNullException("serviceUrl");
            if (urlInfo == null) throw new ArgumentNullException("urlInfo");
            if (methodInfo == null) throw new ArgumentNullException("methodInfo");

            m_serviceMethodId = Guid.NewGuid();
            m_serviceUrl = serviceUrl.Trim();
            m_methodInfo = methodInfo;
            m_urlInfo = urlInfo;
            m_acl = acl;
            m_outputCache = outputCache;
        }

        public Guid ServiceMethodId
        {
            get
            {
                return m_serviceMethodId;
            }
        }

        public string ServiceUrl
        {
            get
            {
                return m_serviceUrl;
            }
        }

        public MethodInfo MethodInfo
        {
            get
            {
                return m_methodInfo;
            }
        }

        public UrlAttribute UrlInfo
        {
            get
            {
                return m_urlInfo;
            }
        }

        public ValidateAclAttribute Acl
        {
            get
            {
                return m_acl;
            }
        }

        public OutputCacheAttribute OutputCache
        {
            get
            {
                return m_outputCache;
            }
        }

        public bool Equals(ServiceMethodMetadata other)
        {
            return Equals(other.m_urlInfo, m_urlInfo) && Equals(other.m_methodInfo, m_methodInfo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ServiceMethodMetadata && Equals((ServiceMethodMetadata) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (m_urlInfo.GetHashCode() * 397) ^ m_methodInfo.GetHashCode();
            }
        }

        public static bool operator ==(ServiceMethodMetadata left, ServiceMethodMetadata right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ServiceMethodMetadata left, ServiceMethodMetadata right)
        {
            return !left.Equals(right);
        }
    }
}
