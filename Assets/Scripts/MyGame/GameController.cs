using System;
using System.Collections.Generic;
using MyGame.DTOs;
using MyGame.NetworkSetup;
using NetworkLib.Common.DTOs;
using NetworkLib.Common.DTOs.MatchMaking;
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
            Debug.Log("Message received " + msg.GetType());

            if (msg is LoginResponse)
            {
                LocalPlayer.LoggedIn = true;
                Players.Add(LocalPlayer.Username, LocalPlayer.gameObject);
                CanvasController.Instance.QueueButton.interactable = true;
                CanvasController.Instance.LoginButton.interactable = false;
            }
            else if (msg is PositionAndYRotation)
            {
                PlayerPositionReceived(msg as PositionAndYRotation);
            }
            else if (msg is MatchMessage)
            {
                //Match found
                LocalPlayer.MatchId = (msg as MatchMessage).MatchId;
            }
            else if (msg is QueueMessage)
            {
                CanvasController.Instance.QueueButton.interactable = false;
            }
            else if (msg is PlayerLeft)
            {
                PlayerLeft pl = msg as PlayerLeft;
                if (Players.ContainsKey(pl.Username))
                {
                    Destroy(Players[pl.Username]);
                    Players.Remove(pl.Username);
                }
            }
            else if (msg is PlayerJoined)
            {
                Debug.Log("Player joined");
            }
            else
            {
                Debug.LogWarning("Unknown message type " + msg.GetType());
            }
        }

        private void PlayerPositionReceived(PositionAndYRotation pos)
        {
            try
            {
                if (Players.ContainsKey(pos.Username))
                {
                    if (pos.Username != LocalPlayer.Username)
                    {
                        Players[pos.Username].GetComponent<Player>().LerpMovement(new Vector3(pos.X, pos.Y, pos.Z));
                    }
                }
                else
                {
                    SpawnNewPlayer(pos);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        private void SpawnNewPlayer(PositionAndYRotation posAndYRotation)
        {
            GameObject go = Instantiate(Resources.Load(CONSTANTS.PlayerPrefab), new Vector3(posAndYRotation.X, posAndYRotation.Y, posAndYRotation.Z), Quaternion.identity) as GameObject;
            Players.Add(posAndYRotation.Username, go);
            Player p = go.GetComponent<Player>();
            p.Username = posAndYRotation.Username;
        }
    }
}