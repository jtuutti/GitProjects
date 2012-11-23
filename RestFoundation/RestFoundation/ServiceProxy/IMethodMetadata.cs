using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Serialization;

namespace RestFoundation.ServiceProxy
{
    public interface IMethodMetadata
    {
        IMethodMetadata SetAuthentication(AuthenticationType type);
        IMethodMetadata SetAuthentication(AuthenticationType type, string defaultUserName);
        IMethodMetadata SetAuthentication(AuthenticationType type, string defaultUserName, string relativeUrlToMatch);
        IMethodMetadata SetDescription(string operationDescription);
        IMethodMetadata SetHeader(ProxyHeader header);
        IMethodMetadata SetHeader(string name, string value);
        IMethodMetadata SetHeaders(IList<ProxyHeader> headers);
        IMethodMetadata SetHidden();
        IMethodMetadata SetHttps();
        IMethodMetadata SetHttps(int port);
        IMethodMetadata SetIPFiltered();
        IMethodMetadata SetQueryParameter(string name, Type type);
        IMethodMetadata SetQueryParameter(string name, Type type, object exampleValue);
        IMethodMetadata SetQueryParameter(string name, Type type, object exampleValue, IList<string> allowedValues);
        IMethodMetadata SetQueryParameter(string name, Type type, object exampleValue, string regexConstraint);
        IMethodMetadata SetQueryParameter(string name, Type type, object exampleValue, IList<string> allowedValues, string regexConstraint);
        IMethodMetadata SetRouteParameter(string name);
        IMethodMetadata SetRouteParameter(string name, object exampleValue);
        IMethodMetadata SetRouteParameter(string name, object exampleValue, IList<string> allowedValues);
        IMethodMetadata SetRequestResourceExample(object instance);
        IMethodMetadata SetRequestResourceExample(object instance, XmlSchemas xmlSchemas);
        IMethodMetadata SetResponseResourceExample(object instance);
        IMethodMetadata SetResponseResourceExample(object instance, XmlSchemas xmlSchemas);
        IMethodMetadata SetResponseStatus(ProxyStatus status);
        IMethodMetadata SetResponseStatus(HttpStatusCode statusCode);
        IMethodMetadata SetResponseStatus(HttpStatusCode statusCode, string statusDescription);
        IMethodMetadata SetResponseStatuses(IList<ProxyStatus> statuses);
    }
}
