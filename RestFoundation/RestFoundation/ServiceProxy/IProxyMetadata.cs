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
        ResourceExampleMetadata GetRequestResourceExample(MethodInfo serviceMethod);
        ResourceExampleMetadata GetResponseResourceExample(MethodInfo serviceMethod);

        ParameterMetadata GetParameter(MethodInfo serviceMethod, string name, bool isRouteParameter);
        IList<ParameterMetadata> GetParameters(MethodInfo serviceMethod, bool isRouteParameter);

        IList<HeaderMetadata> GetHeaders(MethodInfo serviceMethod);
        IList<StatusCodeMetadata> GetResponseStatuses(MethodInfo serviceMethod);

        void Initialize();
    }
}
