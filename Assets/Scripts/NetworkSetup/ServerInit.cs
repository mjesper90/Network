using NetworkLib.GameServer;
using UnityEngine;

namespace MyGame.NetworkSetup
{
    public class ServerInit : MonoBehaviour
    {
        private Server _server;
        private float _cooldown;
        public int Port = CONSTANTS.Port;

        // Start is called before the first frame update
        void Start()
        {
#if UNITY_EDITOR
            _server = new Server(new UnityLogger(), Port);
            GameController.Instance.SetServer(_server);
            Debug.Log("Server started");
#else

#endif
        }

        void FixedUpdate()
        {
            if (_server != null && _cooldown <= 0)
            {
                _server.UpdateServer();
                _cooldown = CONSTANTS.ServerSpeed;
            }
            else
            {
                _cooldown -= Time.deltaTime;
            }
        }

        void OnApplicationQuit()
        {
            if (_server != null)
                _server.Shutdown();
            Debug.Log("Server stopped");
        }
    }
}