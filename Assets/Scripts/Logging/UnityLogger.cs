using UnityEngine;
namespace NetworkLib.Logger
{
    public class UnityLogger : MonoBehaviour, ILogNetwork
    {
        private static UnityLogger _instance;
        private static readonly object lockObject = new object();

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public ILogNetwork Instance()
        {
            return _instance;
        }

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