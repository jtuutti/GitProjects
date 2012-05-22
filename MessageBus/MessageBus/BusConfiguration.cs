using System;
using System.Collections.Generic;
using System.Linq;
using MessageBus.Loggers;

namespace MessageBus
{
    public sealed class BusConfiguration
    {
        private static readonly BusConfiguration configure = new BusConfiguration();

        private readonly Dictionary<Type, IMessageHandler> registeredHandlers = new Dictionary<Type, IMessageHandler>();
        private Func<Type, object> messageCreator = Activator.CreateInstance;

        private BusConfiguration()
        {
            Logger = new ConsoleLogger();
        }

        public static BusConfiguration Configure
        {
            get
            {
                return configure;
            }
        }

        public IBusLogger Logger { get; private set; }

        public Func<Type, object> MessageCreator
        {
            get
            {
                return messageCreator;
            }
            set
            {
                if (value == null)
                {
                    value = Activator.CreateInstance;
                }

                messageCreator = value;
            }
        }
        
        public static Type[] GetHandledTypes()
        {
            return Configure.registeredHandlers.Keys.ToArray();
        }

        public void RegisterHandlers(params IMessageHandler[] handlers)
        {
            if (handlers == null) throw new ArgumentNullException("handlers");

            for (int i = 0; i < handlers.Length; i++)
            {
                IMessageHandler handler = handlers[i];

                if (handler != null)
                {
                    RegisterHandler(handler);
                }
            }
        }

        public IMessageHandler GetHandler(Type messageType)
        {
            if (messageType == null) throw new ArgumentNullException("messageType");

            IMessageHandler handler;
            return registeredHandlers.TryGetValue(messageType, out handler) ? handler : null;
        }

        public BusConfiguration SetLogger(IBusLogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");

            Logger = logger;
            return this;
        }

        private void RegisterHandler(IMessageHandler handler)
        {
            if (handler.MessageType == null) throw new ArgumentException("No message type is defined by the handler", "handler");

            MessageTypeValidator.Validate(handler.MessageType);

            if (registeredHandlers.ContainsKey(handler.MessageType))
            {
                throw new InvalidOperationException(String.Format("There is already a registered handler for the messages of type '{0}'", handler.MessageType.Name));
            }

            registeredHandlers.Add(handler.MessageType, handler);
        }
    }
}
