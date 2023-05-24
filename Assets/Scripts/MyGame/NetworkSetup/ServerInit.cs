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
                Instance = this;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
#if UNITY_EDITOR
            Server = new Server(new UnityLogger("Server::"), Port, new NotQuakeMatch());
            Debug.Log("Server started");
#else

#endif
        }

        void FixedUpdate()
        {
            if (Server != null && _cooldown <= 0)
            {
                Server.UpdateServer();
                _cooldown = CONSTANTS.ServerSpeed;
            }
            else
            {
                _cooldown -= Time.deltaTime;
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