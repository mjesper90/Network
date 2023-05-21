using System.Collections;
using System.Collections.Generic;
using GameServer;
using UnityEngine;

public class ServerInit : MonoBehaviour
{
    private Server _server;
    private float cooldown;
    public int Port = CONSTANTS.Port;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        _server = new Server(Port);
        GameController.Instance.SetServer(_server);
        Debug.Log("Server started");
#else

#endif
    }

    void FixedUpdate()
    {
        if (_server != null && cooldown <= 0)
        {
            _server.UpdateServer();
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
