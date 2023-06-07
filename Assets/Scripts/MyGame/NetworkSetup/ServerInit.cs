using NetworkLib.GameServer;
using UnityEngine;

namespace MyGame.NetworkSetup
{
    public class ServerInit : MonoBehaviour
    {
        public Server Server;
        private float _cooldown;
        public int Port = CONSTANTS.Port;

        public static ServerInit Instance;

        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
#if UNITY_EDITOR
                NotQuakeMatch match = new NotQuakeMatch();
                Server = new Server(new UnityLogger("Server::"), Port, match);
                Server.StartAcceptingClients();
                Debug.Log("Server started");
#else

#endif
                Instance = this;
            }
        }

        void OnApplicationQuit()
        {
            if (Server != null)
                Server.Shutdown();
            Debug.Log("Server stopped");
        }
    }
}