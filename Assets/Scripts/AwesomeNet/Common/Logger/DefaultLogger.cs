using System;

namespace NetworkLib.Common.Logger
{
    public class DefaultLogger : ILogNetwork
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void LogWarning(string message)
        {
            Console.WriteLine(message);
        }

        public void LogError(string message)
        {
            Console.WriteLine(message);
        }
    }
}