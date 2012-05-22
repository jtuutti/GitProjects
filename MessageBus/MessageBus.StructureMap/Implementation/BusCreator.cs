using System;
using StructureMap;

namespace MessageBus.Implementation
{
    internal sealed class BusCreator : IBusCreator
    {
        private readonly IContainer container;

        public BusCreator(IContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");

            this.container = container;
        }

        public IBus CreateBus()
        {
            return container.GetInstance<IBus>();
        }
    }
}
