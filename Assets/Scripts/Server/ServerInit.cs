using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameServer
{
    public class ServerInit : MonoBehaviour
    {
        private Server _server;
        public int Port = 8052;

        // Start is called before the first frame update
        void Start()
        {
#if UNITY_EDITOR
            _server = new Server(Port);
            GameController.Instance.SetServer(_server);
            Debug.Log("Server started");
#endif
        }

        float cooldown;
        void FixedUpdate()
        {
            if (_server != null && cooldown <= 0)
            {
                _server.Update();
                cooldown = CONSTANTS.ServerSpeed;
            }
            else
            {
                cooldown -= Time.deltaTime;
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