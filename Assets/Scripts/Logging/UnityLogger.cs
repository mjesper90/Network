using NetworkLib.Common.Logger;
using UnityEngine;
namespace MyGame
{
    public class UnityLogger : ILogNetwork
    {
        private static readonly object lockObject = new object();

        public void Log(string message)
        {
            lock (lockObject)
            {
                Debug.Log(message);
            }
        }

        public void LogWarning(string message)
        {
            lock (lockObject)
            {
                Debug.LogWarning(message);
            }
        }

        public void LogError(string message)
        {
            lock (lockObject)
            {
                Debug.LogError(message);
            }
        }
    }
}