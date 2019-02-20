using System;

namespace VerySeriousEngine.Utils
{
    public class Logger
    {
        public static void Log(string msg)
        {
            Console.WriteLine("VSE: " + msg);
        }

        public static void LogWarning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("VSE Warning: " + msg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void LogError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("VSE ERROR: " + msg);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
