using System;
using System.Collections.Generic;
using AmongUs.DTOs;
using AmongUs.NetworkSetup;
using NetworkLib.Common.DTOs;
using NetworkLib.Common.DTOs.MatchMaking;
using UnityEngine;

namespace AmongUs.GameControl
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;

        //private Client _client;
        public Player LocalPlayer;

        public Dictionary<string, GameObject> Players = new Dictionary<string, GameObject>();
        private Camera _cam;

        public void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            _cam = Camera.main;

#if UNITY_EDITOR
            InitServer();
#else
            InitPlayer();
            InitClient();

#endif
        }

        public void Update()
        {
            while (ClientInit.Instance?.Client?.NetworkHandler.GetQueueSize() > 0)
            {
                if (ClientInit.Instance.Client.NetworkHandler.TryDequeue(out Message msg))
                    HandleMessage(msg);
            }
        }

        private void HandleMessage(Message msg)
        {
            //Debug.Log("Message received " + msg.GetType());

            if (msg is LoginResponse)
            {
                LocalPlayer.LoggedIn = true;
                Players.Add(LocalPlayer.Username, LocalPlayer.gameObject);
                ClientInit.Instance.SendQueue();
            }
            else if (msg is PositionAndYRotation)
            {
                PlayerPositionReceived(msg as PositionAndYRotation);
            }
            else if (msg is MatchMessage)
            {
                MatchMessage mm = msg as MatchMessage;
                LocalPlayer.MatchId = mm.MatchId;
                foreach (string username in mm.Usernames)
                {
                    if (!Players.ContainsKey(username))
                    {
                        SpawnRemotePlayer(username);
                    }
                }
            }
            else if (msg is QueueMessage)
            {
                //Queue prly accepted, ignore
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
                PlayerJoined pj = msg as PlayerJoined;
                SpawnRemotePlayer(pj.Username);
            }
            else
            {
                Debug.LogWarning("Unknown message type " + msg.GetType());
            }
        }

        private void InitClient()
        {
            GameObject go = Instantiate(Resources.Load(CONSTANTS.AmongUsClientPrefab)) as GameObject;
            go.GetComponent<ClientInit>().ConnectClient();
        }

        private void InitPlayer()
        {
            GameObject go = Instantiate(Resources.Load(CONSTANTS.AmongUsPlayerPrefab), new Vector3(0, 0.5f, 0), Quaternion.identity) as GameObject;
            LocalPlayer = go.GetComponent<Player>();
            LocalPlayer.IsLocal = true;
            LocalPlayer.Username = new System.Random().Next(0, 100000).ToString();

            _cam.transform.SetParent(go.transform);
            _cam.transform.localPosition = CONSTANTS.MyShooterCameraOffset;
            _cam.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        private void InitServer()
        {
            Instantiate(Resources.Load(CONSTANTS.AmongUsServerPrefab));
            _cam.transform.localPosition = CONSTANTS.MyShooterServerCamOffset;
            _cam.transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        private void PlayerPositionReceived(PositionAndYRotation pos)
        {
            Debug.Log(pos);
            try
            {
                if (Players.ContainsKey(pos.Username))
                {
                    if (pos.Username != LocalPlayer.Username)
                    {
                        Player p = Players[pos.Username].GetComponent<Player>();
                        p.LerpMovementAndRotation(new Vector3(pos.X, pos.Y, pos.Z), pos.YRotation);
                    }
                }
                else
                {
                    Debug.LogWarning("Player not found " + pos.Username);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        private void SpawnRemotePlayer(string username)
        {
            if (!Players.ContainsKey(username))
            {
                GameObject go = Instantiate(Resources.Load(CONSTANTS.AmongUsPlayerPrefab), new Vector3(0, 0.5f, 0), Quaternion.identity) as GameObject;
                Player p = go.GetComponent<Player>();
                p.Username = username;
                Players.Add(username, go);
            }
        }
    }
}