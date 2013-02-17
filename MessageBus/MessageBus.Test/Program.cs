using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MessageBus.Msmq;
using StructureMap;

namespace MessageBus.Test
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine();

            ObjectFactory.Configure(c => c.For<ITestMessage>().Use<TestMessage>());

            IBus bus = BusConfiguration.Configure
                                            .UseStructureMap(typeof(Program).Assembly)
                                       .CreateBus();

            bus.Advanced.Purge<TestMessage>(PurgeTargets.NonFaultMessages);
            bus.Advanced.Purge<TestMessage2>(PurgeTargets.NonFaultMessages);

            bus.Settings.MinRetryTimeout = TimeSpan.FromSeconds(2);
            bus.Events.FaultOccurred += (sender, args) => DisplayException(args.FaultException);

            bus.SubscribeAll();

            StartProcess(bus).Wait();
        }

        private static async Task StartProcess(IBus bus)
        {
            await SendMessages(bus);

            int i = 0;
            var stopwatch = Stopwatch.StartNew();

            while (true)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("Iteration # {0}", ++i);
                Console.ResetColor();

                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Hit ENTER to resend the messages or Ctrl+Break to exit...");
                Console.ResetColor();
                //Console.ReadLine();
                Console.WriteLine();

                await SendMessages(bus);

                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("-- Seconds elapsed: {0}", stopwatch.Elapsed.TotalSeconds);

                if (stopwatch.Elapsed.TotalSeconds > 0)
                {
                    Console.Write("-- Operations per second: {0}", Convert.ToInt32(i / stopwatch.Elapsed.TotalSeconds));
                }

                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        private static async Task SendMessages(IBus bus)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Sending Messages...");
            Console.ResetColor();
            Console.WriteLine();

            await bus.SendAndReceive<ITestMessage, string>(message =>
            {
                message.ID = 20;
                message.Name = "ABC";
            });

            await bus.SendAndReceive<TestMessage2, string>(message =>
            {
                message.ID = 25;
                message.Name = "EFG";
            });

            await bus.SendAndReceive<TestMessage, string>(message =>
            {
                message.ID = 26;
                message.Name = "ABC";
            });
        }

        private static void DisplayException(Exception ex)
        {
            ConsoleColor consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("Error occurred: {0}", ex);
            Console.WriteLine();
            Console.ForegroundColor = consoleColor;

            Console.ReadKey();
        }
    }

    public interface ITestMessage : IMessage
    {
        int? ID { get; set; }
        string Name { get; set; }
    };

    [Serializable]
    [QueueName("Test")]
    public class TestMessage : MessageBase, ITestMessage, ISaga
    {
        public int? ID { get; set; }
        public string Name { get; set; }

        public TimeSpan Timeout
        {
            get
            {
                return TimeSpan.FromSeconds(30);
            }
        }
    }

    [Serializable]
    [QueueName("TestTwo")]
    public class TestMessage2 : MessageBase
    {
        public int? ID { get; set; }
        public string Name { get; set; }
    }

    public class TestMessageHandler : IMessageHandler
    {
        private readonly IBus bus;

        public TestMessageHandler(IBus bus)
        {
            if (bus == null) throw new ArgumentNullException("bus");

            this.bus = bus;
        }

        public Type MessageType
        {
            get
            {
                return typeof(TestMessage);
            }
        }

        public bool Handle(IMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");

            // Thread.Sleep(100);

            bus.Reply(message, "OK 1");

            BusConfiguration.Configure.Logger.Debug("Message with ID '{0}' of type '{1}' handled", message.Headers[SystemHeaders.MessageID], message.GetType().FullName);

            return true;
        }
    }

    public class TestMessageHandler2 : IMessageHandler
    {
        private readonly IBus bus;

        public TestMessageHandler2(IBus bus)
        {
            if (bus == null) throw new ArgumentNullException("bus");

            this.bus = bus;
        }

        public Type MessageType
        {
            get
            {
                return typeof(TestMessage2);
            }
        }

        public bool Handle(IMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");

            // Thread.Sleep(500);

            bus.Reply(message, "OK 2");

            BusConfiguration.Configure.Logger.Debug("Message with ID '{0}' of type '{1}' handled.", message.Headers[SystemHeaders.MessageID], message.GetType().FullName);

            return true;
        }
    }
}
