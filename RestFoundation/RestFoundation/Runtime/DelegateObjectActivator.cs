using System;

namespace RestFoundation.Runtime
{
    internal sealed class DelegateObjectActivator : IObjectActivator
    {
        private readonly Func<Type, object> m_factory;
        private readonly Action<object> m_builder;

        public DelegateObjectActivator(Func<Type, object> factory, Action<object> builder)
        {
            m_factory = factory;
            m_builder = builder;
        }

        public object Create(Type objectType)
        {
            return m_factory(objectType);
        }

        public void BuildUp(object obj)
        {
            if (obj != null)
            {
                m_builder(obj);
            }
        }
    }
}
