using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beastrack.Utility
{
    public class ErrorHandler
    {
        public static void Terminate(string msg)
        {
            Console.WriteLine($"Error: {msg}");
            Environment.Exit(1);
        }
        public static void Warn(string msg)
        {
            Console.WriteLine($"Warning: {msg}");
        }
        public static void AskTermination(string msg)
        {
            Console.Error.WriteLine(msg);

            while (true)
            {
                Console.WriteLine("Continue? (y/n)");
                string ans = Console.ReadLine();
                if (ans == "n") Environment.Exit(1);
                else if (ans == "y") break;
            }
        }
        public static void Throw(string msg)
        {

        }
    }
}
