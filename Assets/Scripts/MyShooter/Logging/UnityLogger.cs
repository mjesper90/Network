using NetworkLib.Common.Logger;
using UnityEngine;

namespace MyShooter.Logging
{
    public class UnityLogger : ILogNetwork
    {
        private static readonly object lockObject = new object();
        private string _prefix;

        public UnityLogger(string prefix)
        {
            _prefix = prefix;
        }

        public void Log(string message)
        {
            lock (lockObject)
            {
                Debug.Log(_prefix + message);
            }
        }

        public void LogWarning(string message)
        {
            lock (lockObject)
            {
                Debug.LogWarning(_prefix + message);
            }
        }

        public void LogError(string message)
        {
            lock (lockObject)
            {
                Debug.LogError(_prefix + message);
            }
        }
    }
}