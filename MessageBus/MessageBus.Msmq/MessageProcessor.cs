using System;
using System.Messaging;
using System.Threading;
using System.Threading.Tasks;
using MessageBus.Msmq.Helpers;

namespace MessageBus.Msmq
{
    internal sealed class MessageProcessor
    {
        private readonly MsmqBus bus;

        public MessageProcessor(MsmqBus bus)
        {
            this.bus = bus;
        }

        public void ProcessMessage(Type messageType, Task<Message> task)
        {
            if (task.Result == null || String.IsNullOrWhiteSpace(task.Result.Id))
            {
                return;
            }

            var helper = new PendingQueueHelper(bus);
            helper.MoveToSubQueue(messageType, task.Result);

            IMessageHandler handler = BusConfiguration.Configure.GetHandler(messageType);
            bool handled = false;

            if (handler != null)
            {
                var message = task.Result.Body as IMessage;

                if (message != null)
                {
                    handled = HandleMessage(task, handler, message);
                }
                else
                {
                    var messages = task.Result.Body as IMessage[];

                    if (messages != null)
                    {
                        handled = HandleMessages(task, handler, messages);
                    }
                }
            }

            if (handled)
            {
                bus.Advanced.Remove(messageType, task.Result.Id);
            }
            else
            {
                Thread.Sleep(bus.Settings.MinRetryTimeout);
                helper.MoveFromSubQueue(messageType, task.Result);
            }
        }

        private static bool HandleMessage(Task<Message> task, IMessageHandler handler, IMessage message)
        {
            if (message != null)
            {
                message.Headers[SystemHeaders.MessageID] = task.Result.Id;
            }

            return handler.Handle(message);
        }

        private static bool HandleMessages(Task<Message> task, IMessageHandler handler, IMessage[] messages)
        {
            bool messagesHandled = messages.Length == 0;
            if (messagesHandled) return true;

            for (int i = 0; i < messages.Length; i++)
            {
                bool handled = HandleMessage(task, handler, messages[i]);

                if (!messagesHandled && handled)
                {
                    messagesHandled = true;
                }
            }

            return messagesHandled;
        }

        public void ProcessFault(Type messageType, Task<Message> task, Exception ex)
        {
            if (task.Result == null || String.IsNullOrWhiteSpace(task.Result.Id)) return;

            var helper = new FaultQueueHelper(bus);
            helper.MoveToSubQueue(messageType, task.Result);

            ((MsmqBus.BusEvents) bus.Events).OnFaultOccurred(task.Result.Id, task.Result.Body as IMessage, ex);
        }
    }
}
