using System;

namespace MvcAlt.Infrastructure
{
    public class DefaultModelFactory : IModelFactory
    {
        public object Create(Type modelType)
        {
            if (modelType == null) throw new ArgumentNullException("modelType");

            return Activator.CreateInstance(modelType);
        }
    }
}