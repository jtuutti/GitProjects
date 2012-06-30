using System;
using RestFoundation.DataBinders;

namespace RestFoundation
{
    public sealed class DataBinderBuilder
    {
        internal DataBinderBuilder()
        {
        }

        public IDataBinder Get(Type objectType)
        {
            if (objectType == null) throw new ArgumentNullException("objectType");

            return DataBinderRegistry.GetBinder(objectType);
        }

        public void Set(Type objectType, IDataBinder binder)
        {
            if (objectType == null) throw new ArgumentNullException("objectType");
            if (binder == null) throw new ArgumentNullException("binder");

            DataBinderRegistry.SetBinder(objectType, binder);
        }

        public bool Remove(Type objectType)
        {
            if (objectType == null) throw new ArgumentNullException("objectType");

            return DataBinderRegistry.RemoveBinder(objectType);
        }

        public void Clear()
        {
            DataBinderRegistry.ClearBinders();
        }
    }
}
