using System;
using System.Globalization;

namespace MessageBus
{
    public static class MessageTypeValidator
    {
        public static void Validate(Type messageType)
        {
            if (!messageType.IsClass || messageType.IsAbstract || messageType.GetInterface(typeof(IMessage).FullName) == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                                                                  "Invalid message type '{0}' provided. It must be a concrete class that implements the IMessage interface.",
                                                                  messageType.Name));
            }
        }
    }
}
