using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Messaging;
using System.Threading;
using System.Threading.Tasks;
using MessageBus.Msmq.Helpers;

namespace MessageBus.Msmq
{
    internal sealed class MessageListener
    {
        private readonly ConcurrentDictionary<Type, CancellationTokenSource> cancellations = new ConcurrentDictionary<Type, CancellationTokenSource>();
        private readonly HashSet<string> queueNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly MsmqBus bus;

        public MessageListener(MsmqBus bus)
        {
            this.bus = bus;
        }

        public async Task Start(Type messageType)
        {
            if (messageType == null) throw new ArgumentNullException("messageType");

            MessageTypeValidator.Validate(messageType);

            if (cancellations.ContainsKey(messageType))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                                                                  "Message of type '{0}' is already subscribed to the bus",
                                                                  messageType.Name));
            }

            string queueName = QueueNameHelper.GetName(messageType);

            if (!queueNames.Add(queueName))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                                                                  "A message with queue name '{0}' is already subscribed to the bus",
                                                                  queueName.ToLowerInvariant()));
            }

            await Listen(messageType);
        }

        public void Stop(Type messageType)
        {           
            CancellationTokenSource cancellationSource;

            if (!cancellations.TryRemove(messageType, out cancellationSource))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                                                                  "Message of type '{0}' was not subscribed to the bus",
                                                                  messageType.Name));
            }

            cancellationSource.Cancel();

            queueNames.Remove(QueueNameHelper.GetName(messageType));
        }

        public void StopAll()
        {
            var typesToStop = new List<Type>(cancellations.Keys);

            for (int i = 0; i < typesToStop.Count; i++)
            {
                Stop(typesToStop[i]);
            }
        }

        private async Task Listen(Type messageType)
        {
            var cancellationSource = new CancellationTokenSource();
            cancellations.TryAdd(messageType, cancellationSource);

            var processor = new MessageProcessor(bus);

            Task<Message> task = Task.Run(() => !cancellationSource.Token.IsCancellationRequested ? GetFromQueue(messageType) : null, cancellationSource.Token);

            try
            {
                await task;

                processor.ProcessMessage(messageType, task);
            }
            catch (Exception ex)
            {
                try
                {
                    processor.ProcessFault(messageType, task, ex);
                }
                catch
                {
                    Thread.Sleep(bus.Settings.MinRetryTimeout);
                }
            }
            finally
            {               
                CancellationTokenSource removedCancellationSource;
                cancellations.TryRemove(messageType, out removedCancellationSource);
            }

            await Listen(messageType);
        }

        private Message GetFromQueue(Type messageType)
        {
            ((MsmqBus.BusEvents) bus.Events).OnListening(messageType);

            try
            {
                var helper = new InputQueueHelper(bus);

                using (var queue = helper.GetQueue(messageType))
                {
                    return queue.Peek(MessageQueue.InfiniteTimeout);
                }
            }
            catch (MessageQueueException)
            {
                return null;
            }
        }
    }
}
