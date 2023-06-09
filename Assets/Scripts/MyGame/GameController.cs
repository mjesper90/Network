using System;
using System.Collections.Generic;
using MyGame.DTOs;
using MyGame.NetworkSetup;
using NetworkLib.Common.DTOs;
using NetworkLib.GameClient;
using NetworkLib.GameServer;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
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
            GameObject go_server = Instantiate(Resources.Load(CONSTANTS.ServerPrefab), Vector3.zero, Quaternion.identity) as GameObject;
            _server = go_server.GetComponent<ServerInit>().Server;
            if (_server != null)
                return;

            GameObject go = Instantiate(Resources.Load(CONSTANTS.PlayerPrefab), new Vector3(0, 0.5f, 0), Quaternion.identity) as GameObject;
            GameObject go_client = Instantiate(Resources.Load(CONSTANTS.ClientPrefab), Vector3.zero, Quaternion.identity) as GameObject;
            _cam = Camera.main;

            //Add camera to player with offset
            _cam.transform.SetParent(go.transform);
            _cam.transform.localPosition = CONSTANTS.CameraOffset;

            LocalPlayer = go.GetComponent<Player>();
            LocalPlayer.IsLocal = true;
        }

        public void Update()
        {
            if (_client == null)
                _client = ClientInit.Instance?.Client;
            if (_client == null)
                return;
            CanvasController.Instance.OptionsButton.GetComponentInChildren<Text>().text = _client?.NetworkHandler.GetQueueSize().ToString();
            while (_client?.NetworkHandler.GetQueueSize() > 0)
            {
                if (_client.NetworkHandler.TryDequeue(out Message msg))
                    HandleMessage(msg);
            }
        }

        private void HandleMessage(Message msg)
        {
            Debug.Log("Message received " + msg.MsgType);
            switch (msg.MsgType)
            {
                case MessageType.LoginResponse:
                    LocalPlayer.LoggedIn = true;
                    Players.Add(LocalPlayer.GetUser().Username, LocalPlayer.gameObject);
                    CanvasController.Instance.QueueButton.interactable = true;
                    CanvasController.Instance.LoginButton.interactable = false;
                    break;
                case MessageType.Message:
                    Debug.Log("Message " + _client.MsgFactory.Deserialize<string>(msg.Data));
                    break;
                case MessageType.User:
                    User user = _client.MsgFactory.Deserialize<User>(msg.Data);
                    UserReceived(user);
                    break;
                case MessageType.PlayerPosition:
                    PlayerPosition player = _client.MsgFactory.Deserialize<PlayerPosition>(msg.Data);
                    PlayerPositionReceived(player);
                    break;
                case MessageType.MatchJoined:
                    CanvasController.Instance.QueueButton.interactable = false;
                    LocalPlayer.InGame = true;
                    break;
                case MessageType.PlayerRotation:
                    PlayerRotation playerRotation = _client.MsgFactory.Deserialize<PlayerRotation>(msg.Data);
                    PlayerRotationReceived(playerRotation);
                    break;
                default:
                    Debug.LogWarning("Unknown message type " + msg.MsgType);
                    break;
            }
        }

        private void PlayerRotationReceived(PlayerRotation pr)
        {
            if (Players.ContainsKey(pr.Username))
            {
                if (pr.Username != LocalPlayer.GetUser().Username)
                {
                    Players[pr.Username].GetComponent<Player>().LerpRotation(pr.Y);
                }
            }
        }

        private void PlayerPositionReceived(PlayerPosition user)
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
                    p.SetUser(new User(user.Username));
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        private void UserReceived(User user)
        {
            Players[user.Username].GetComponent<Player>().SetUser(user);
        }
    }
}