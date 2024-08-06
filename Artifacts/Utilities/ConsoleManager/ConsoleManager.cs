using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifacts.Utilities.ConsoleManager
{
    internal static class ConsoleManager
    {

        public static ConsoleColor defaultConsoleColor = ConsoleColor.Green; //Console.ForegroundColor;
        public static ConsoleColor errorConsoleColor = ConsoleColor.Red;
        public static void Write(string message, ConsoleColor? consoleColor = null)
        {
            if (consoleColor == ConsoleColor.Red)
            {
                Console.WriteLine("=> erreur");
            }
            ConsoleColor defaultColor = Console.ForegroundColor;
            if (consoleColor.HasValue)
            {
                Console.ForegroundColor = consoleColor.Value;
            }

            Console.WriteLine(message);

            if (consoleColor.HasValue)
            {
                Console.ForegroundColor = defaultColor;  
            }
        }

        internal static void SetColor(ConsoleColor defaultConsoleColor)
        {
            Console.ForegroundColor = defaultConsoleColor;
        }
    }
}
