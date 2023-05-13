using System.Linq;
using System.Net.Sockets;
using DTOs;
using GameClient;
using UnityEngine;


public class ClientInit : MonoBehaviour
{
    public string IP = "127.0.0.1";
    public int Port = 8052;
    private Client _client;
    private Player _localPlayer;

    public static ClientInit Instance;

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

    public void Send(object obj)
    {
        _client.Send(obj);
    }

    protected void FixedUpdate()
    {
        if (_client == null && GameController.Instance.LocalPlayer != null)
        {
            _localPlayer = GameController.Instance.LocalPlayer;
            Debug.Log("Local player found " + _localPlayer.name);

            if (_localPlayer?.GetUser() != null)
            {
                _client = new Client(IP, Port);
                Debug.Log("Client started");
                _client.Send(_localPlayer.GetUser());
                GameController.Instance.SetClient(_client);
            }
            else
            {
                Debug.Log("Local player not found");
            }
        }
        else if (_client != null)
        {
            _client.Send(new Position(_localPlayer.transform.position));
        }
        else
        {
            Debug.Log("Client not connected");
        }
    }
}
