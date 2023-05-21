using System;
using System.Collections.Generic;
using DTOs;
using NetworkLib;
using NetworkLib.GameClient;
using NetworkLib.GameServer;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    private Camera _cam;

    public Dictionary<string, GameObject> Players = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> Projectiles = new Dictionary<string, GameObject>();
    private Server _server;
    private Client _client;
    public Player LocalPlayer;

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

    public void Start()
    {
        GameObject go = Instantiate(Resources.Load(CONSTANTS.PlayerPrefab), new Vector3(0, 0.5f, 0), Quaternion.identity) as GameObject;
        _cam = Camera.main;

        //Add camera to player with offset
        _cam.transform.SetParent(go.transform);
        _cam.transform.localPosition = CONSTANTS.CameraOffset;

        LocalPlayer = go.GetComponent<Player>();
        Debug.Log(LocalPlayer.GetUser().Username + " spawned");
        Players.Add(LocalPlayer.GetUser().Username, go);
        LocalPlayer.IsLocal = true;
    }

    public void SetServer(Server server)
    {
        _server = server;
    }

    public void SetClient(Client client)
    {
        _client = client;
    }

    private void PlayerReceived(PlayerPosition user)
    {
        try
        {
            if (Players.ContainsKey(user.Username))
            {
                if (user.Username != LocalPlayer.GetUser().Username)
                {
                    Players[user.Username].GetComponent<Player>().LerpMovement(new Vector3(user.Pos.X, user.Pos.Y, user.Pos.Z));
                }
            }
            else
            {
                GameObject go = Instantiate(Resources.Load(CONSTANTS.PlayerPrefab), new Vector3(user.Pos.X, user.Pos.Y, user.Pos.Z), Quaternion.identity) as GameObject;
                Players.Add(user.Username, go);
                Player p = go.GetComponent<Player>();
                p.SetUser(new User(user.ID, user.Username));
                Debug.Log("Player " + user.Username + " spawned");
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public Server GetServer()
    {
        if (_server == null)
        {
            Debug.Log("Server is null");
            return null;
        }
        return _server;
    }

    public Client GetClient()
    {
        if (_client == null)
        {
            Debug.Log("Client is null");
            return null;
        }
        return _client;
    }

    public void Update()
    {
        while (_client.NetworkHandler.MessageQueue.Count > 0)
        {
            _client.NetworkHandler.MessageQueue.TryDequeue(out Message msg);
            HandleMessage(msg);
        }
    }

    private void HandleMessage(Message msg)
    {
        switch (msg.MsgType)
        {
            case MessageType.LoginResponse:
                Debug.Log("Login response received");
                LocalPlayer.LoggedIn = true;
                _client.Send(new Message(MessageType.JoinQueue, new byte[0], ""));
                break;
            case MessageType.Message:
                Debug.Log("Message received " + _client.Deserialize<string>(msg.Data));
                break;
            case MessageType.User:
                User user = _client.Deserialize<User>(msg.Data);
                UserReceived(user);
                break;
            case MessageType.PlayerPosition:
                PlayerPosition player = _client.Deserialize<PlayerPosition>(msg.Data);
                PlayerReceived(player);
                break;
            case MessageType.MatchJoined:
                LocalPlayer.InGame = true;
                break;
            default:
                Debug.Log("Unknown message type");
                break;
        }
    }

    private void UserReceived(User user)
    {
        Players[user.Username].GetComponent<Player>().SetUser(user);
    }
}
