using System;

namespace RestFoundation.Client
{
    public interface IRestSerializerFactory
    {
        IRestSerializer Create(Type objectType, RestResourceType resourceType);
    }
}
