using System;
using System.Threading;
using MessageBus.Mvc.Messages;

namespace MessageBus.Mvc.Host
{
    public class CommandMessageHandler : IMessageHandler
    {
        public IBus Bus { get; set; }

        public Type MessageType
        {
            get
            {
                return typeof(Command);
            }
        }

        public bool Handle(IMessage message)
        {
            var commandMessage = message as Command;
            if (commandMessage == null) return false;

            string messageId = commandMessage.Headers[SystemHeaders.MessageID];

            Console.WriteLine("{0} - Command message # {1} received", DateTime.Now, messageId);
            Thread.Sleep(TimeSpan.FromSeconds(1));

            var response = (commandMessage.Id % 2 == 0) ? MessageTypeEnum.Even : MessageTypeEnum.Odd;
            Bus.Reply(message, response);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("{0} - Command message # {1} processed", DateTime.Now, messageId);
            Console.ResetColor();

            return true;
        }
    }
}
