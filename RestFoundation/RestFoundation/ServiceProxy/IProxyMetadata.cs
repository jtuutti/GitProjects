using System.Collections.Generic;
using System.Reflection;

namespace RestFoundation.ServiceProxy
{
    public interface IProxyMetadata
    {
        bool IsIPFiltered(MethodInfo serviceMethod);
        bool IsHidden(MethodInfo serviceMethod);

        string GetDescription(MethodInfo serviceMethod);
        AuthenticationMetadata GetAuthentication(MethodInfo serviceMethod);
        HttpsMetadata GetHttps(MethodInfo serviceMethod);
        ResourceBuilderMetadata GetRequestResourceBuilder(MethodInfo serviceMethod);
        ResourceBuilderMetadata GetResponseResourceBuilder(MethodInfo serviceMethod);

        IList<HeaderMetadata> GetHeaders(MethodInfo serviceMethod);
        IList<ParameterMetadata> GetParameters(MethodInfo serviceMethod);
        IList<StatusCodeMetadata> GetStatusCodes(MethodInfo serviceMethod);

        void Initialize();
    }
}