using System;

namespace MessageBus.Mvc.Host
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("Starting Message Bus...");

            IBus bus = BusConfiguration.Configure.UseStructureMap(typeof(Program).Assembly).CreateBus();

            // Optionally purge all command messages
            // bus.Advanced.Purge<Messages.Command>(PurgeTargets.AllMessages);

            bus.Events.FaultOccurred += (sender, args) => DisplayException(args.FaultException);

            bus.SubscribeAll();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press Ctrl+Break to exit...");
            Console.ResetColor();

            while (true)
            {
            }
        }

        private static void DisplayException(Exception ex)
        {
            ConsoleColor consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("Error occurred: {0}", ex);
            Console.WriteLine();
            Console.ForegroundColor = consoleColor;
        }
    }
}
