using System;

namespace MvcAlt
{
    public interface IModelFactory
    {
        object Create(Type modelType);
    }
}
