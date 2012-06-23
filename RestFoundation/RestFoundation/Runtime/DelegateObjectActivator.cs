using System;

namespace RestFoundation.Runtime
{
    internal sealed class DelegateObjectActivator : IObjectActivator
    {
        private readonly Func<Type, object> m_factory;

        public DelegateObjectActivator(Func<Type, object> factory)
        {
            m_factory = factory;
        }

        public object Create(Type objectType)
        {
            return m_factory(objectType);
        }
    }
}
