using System.Net.Sockets;
using DTOs;
using NetworkLib;
using NetworkLib.GameClient;
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
        if (_client?.IsConnected() == true && _localPlayer?.InGame == true)
        {
            PlayerPosition player = new PlayerPosition(_localPlayer.GetUser().Username, _localPlayer.transform.position.x, _localPlayer.transform.position.y, _localPlayer.transform.position.z);
            _client.Send(new Message(MessageType.PlayerPosition, _client.Serialize(player), ""));
        }
        else
        {
            TryLogin();
        }
    }

    private void TryLogin()
    {
        if (_client == null && GameController.Instance.LocalPlayer != null)
        {
            _localPlayer = GameController.Instance.LocalPlayer;
            Debug.Log("Local player found " + _localPlayer.name);

            if (_localPlayer?.GetUser() != null)
            {
                _client = new Client(new TcpClient(IP, Port));
                Debug.Log("Client started");
                GameController.Instance.SetClient(_client);
                _client.Send(new Message(MessageType.Login, _client.Serialize(_localPlayer.GetUser()), ""));
            }
            else
            {
                Debug.Log("Local player not found");
            }
        }
    }
}
