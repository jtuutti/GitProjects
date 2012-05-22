using System;

namespace MessageBus.Loggers
{
    public class ConsoleLogger : IBusLogger
    {
        public void Debug(string info, params object[] args)
        {
            Log(ConsoleColor.DarkCyan, info, args);
        }

        public void Error(string info, params object[] args)
        {
            Log(ConsoleColor.Red, info, args);
        }

        public void Info(string info, params object[] args)
        {
            Log(ConsoleColor.Gray, info, args);
        }

        public void Warning(string info, params object[] args)
        {
            Log(ConsoleColor.Yellow, info, args);
        }

        protected static void Log(ConsoleColor color, string info, object[] args)
        {
            if (info == null) return;

            ConsoleColor previousColor = Console.ForegroundColor;

            try
            {
                Console.ForegroundColor = color;
                Console.Write("{0} - ", DateTime.Now);

                if (args != null && args.Length > 0)
                {
                    Console.WriteLine(info, args);
                }
                else
                {
                    Console.WriteLine(info);
                }
            }
            finally
            {
                Console.ForegroundColor = previousColor;
            }
        }
    }
}
