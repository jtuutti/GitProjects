// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Reflection;
using RestFoundation.ServiceProxy.OperationMetadata;

namespace RestFoundation.ServiceProxy
{
    internal sealed class NullProxyMetadata : IProxyMetadata, IEquatable<NullProxyMetadata>
    {
        public bool IsIPFiltered(MethodInfo serviceMethod)
        {
            return false;
        }

        public bool IsHidden(MethodInfo serviceMethod)
        {
            return false;
        }

        public bool HasJsonSupport(MethodInfo serviceMethod)
        {
            return false;
        }

        public bool HasXmlSupport(MethodInfo serviceMethod)
        {
            return false;
        }

        public string GetDescription(MethodInfo serviceMethod)
        {
            return null;
        }

        public string GetLongDescription(MethodInfo serviceMethod)
        {
            return null;
        }

        public AuthenticationMetadata GetAuthentication(MethodInfo serviceMethod)
        {
            return null;
        }

        public HttpsMetadata GetHttps(MethodInfo serviceMethod)
        {
            return null;
        }

        public ResourceExampleMetadata GetRequestResourceExample(MethodInfo serviceMethod)
        {
            return null;
        }

        public ResourceExampleMetadata GetResponseResourceExample(MethodInfo serviceMethod)
        {
            return null;
        }

        public ParameterMetadata GetParameter(MethodInfo serviceMethod, string name, bool isRouteParameter)
        {
            return null;
        }

        public IList<ParameterMetadata> GetParameters(MethodInfo serviceMethod, bool isRouteParameter)
        {
            return null;
        }

        public IList<HeaderMetadata> GetHeaders(MethodInfo serviceMethod)
        {
            return new List<HeaderMetadata>();
        }

        public IList<StatusCodeMetadata> GetResponseStatuses(MethodInfo serviceMethod)
        {
            return new List<StatusCodeMetadata>();
        }

        public void Initialize()
        {
        }

        public bool Equals(NullProxyMetadata other)
        {
            return !ReferenceEquals(null, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is NullProxyMetadata;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
