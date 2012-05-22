using System;
using System.Globalization;
using System.Messaging;
using System.Runtime.InteropServices;

namespace MessageBus.Msmq
{
    internal static class MsmqExtensions
    {
        private const char SubQueueSeparator = ';';

        public static void MoveFromSubQueue(this MessageQueue queue, Message message)
        {
            ValidateParameters(queue, queue.QueueName, message);

            int separatorIndex = queue.QueueName.IndexOf(SubQueueSeparator);
            if (separatorIndex <= 0) throw new ArgumentException("Message queue provided is not a subqueue: " + queue.QueueName);

            string fullQueueName = String.Format(CultureInfo.InvariantCulture, @"DIRECT=OS:.\{0}", queue.QueueName.Substring(0, separatorIndex));
            int error = MoveMessage(queue, message, fullQueueName);

            if (error != 0)
            {
                throw CreateInvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot close the queue '{0}'", fullQueueName), error);
            }
        }

        public static void MoveFromSubQueue(this MessageQueue queue, Message message, string subQueueName)
        {
            ValidateParameters(queue, subQueueName, message);

            int separatorIndex = queue.QueueName.IndexOf(SubQueueSeparator);
            if (separatorIndex <= 0) throw new ArgumentException("Message queue provided is not a subqueue: " + queue.QueueName);

            string fullQueueName = String.Format(CultureInfo.InvariantCulture, @"DIRECT=OS:.\{0};{1}", queue.QueueName.Substring(0, separatorIndex), subQueueName);
            int error = MoveMessage(queue, message, fullQueueName);

            if (error != 0)
            {
                throw CreateInvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot close the queue '{0}'", fullQueueName), error);
            }
        }

        public static void MoveToSubQueue(this MessageQueue queue, string subQueueName, Message message)
        {
            ValidateParameters(queue, subQueueName, message);

            string queueName = queue.QueueName;
            int separatorIndex = queueName.IndexOf(SubQueueSeparator);
            if (separatorIndex > 0)
                queueName = queueName.Substring(0, separatorIndex);

            string fullQueueName = String.Format(CultureInfo.InvariantCulture, @"DIRECT=OS:.\{0};{1}", queueName, subQueueName);
            int error = MoveMessage(queue, message, fullQueueName);

            if (error != 0)
            {
                throw CreateInvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot close the queue '{0}'", fullQueueName), error);
            }
        }

        private static void ValidateParameters(MessageQueue queue, string subQueueName, Message message)
        {
            if (queue == null) throw new ArgumentNullException("queue");
            if (String.IsNullOrWhiteSpace(subQueueName)) throw new ArgumentNullException("subQueueName");
            if (message == null) throw new ArgumentNullException("message");
        }

        private static int MoveMessage(MessageQueue queue, Message message, string fullQueueName)
        {
            IntPtr queueHandle = IntPtr.Zero;

            var error = NativeMethods.MQOpenQueue(fullQueueName, NativeMethods.MQ_MOVE_ACCESS, NativeMethods.MQ_DENY_NONE, ref queueHandle);

            if (error != 0)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot open the queue '{0}'", fullQueueName), Marshal.GetExceptionForHR(error));
            }

            try
            {
                error = NativeMethods.MQMoveMessage(queue.ReadHandle, queueHandle, message.LookupId, null);

                if (error != 0)
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot move a message to the queue '{0}'", fullQueueName), Marshal.GetExceptionForHR(error));
                }
            }
            finally
            {
                error = NativeMethods.MQCloseQueue(queueHandle);
            }

            return error;
        }

        private static InvalidOperationException CreateInvalidOperationException(string message, int error)
        {
            return new InvalidOperationException(message, Marshal.GetExceptionForHR(error));
        }
    }
}
