using System;
using RestFoundation.Runtime;

namespace RestFoundation
{
    public sealed class ObjectTypeBinderBuilder
    {
        internal ObjectTypeBinderBuilder()
        {
        }

        public IObjectTypeBinder Get(Type objectType)
        {
            if (objectType == null) throw new ArgumentNullException("objectType");

            return ObjectTypeBinderRegistry.GetBinder(objectType);
        }

        public void Set(Type objectType, IObjectTypeBinder binder)
        {
            if (objectType == null) throw new ArgumentNullException("objectType");
            if (binder == null) throw new ArgumentNullException("binder");

            ObjectTypeBinderRegistry.SetBinder(objectType, binder);
        }

        public bool Remove(Type objectType)
        {
            if (objectType == null) throw new ArgumentNullException("objectType");

            return ObjectTypeBinderRegistry.RemoveBinder(objectType);
        }

        public void Clear()
        {
            ObjectTypeBinderRegistry.ClearBinders();
        }
    }
}
