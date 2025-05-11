using System;

namespace AnyCompanyBankingProduct.Services
{
    /// <summary>
    /// Platform-agnostic interface for console operations
    /// </summary>
    public interface IConsoleService
    {
        void WriteLine(string message);
        void Write(string message);
        void WriteLineWithColor(string message, ConsoleColor color);
        void WriteWithColor(string message, ConsoleColor color);
        string ReadLine();
        void Clear();
    }

    /// <summary>
    /// Implementation of IConsoleService for .NET Framework
    /// </summary>
    public class ConsoleService : IConsoleService
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void Write(string message)
        {
            Console.Write(message);
        }

        public void WriteLineWithColor(string message, ConsoleColor color)
        {
            try
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        public void WriteWithColor(string message, ConsoleColor color)
        {
            try
            {
                Console.ForegroundColor = color;
                Console.Write(message);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public void Clear()
        {
            Console.Clear();
        }
    }
}
