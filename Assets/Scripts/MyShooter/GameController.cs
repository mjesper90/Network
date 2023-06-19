using System;
using System.Collections.Generic;
using MyShooter.DTOs;
using MyShooter.NetworkSetup;
using MyShooter.UI;
using NetworkLib.Common.DTOs;
using NetworkLib.Common.DTOs.MatchMaking;
using UnityEngine;

namespace MyShooter.GameControl
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;
        private Camera _cam;

        public Dictionary<string, GameObject> Players = new Dictionary<string, GameObject>();
        public Dictionary<string, GameObject> Projectiles = new Dictionary<string, GameObject>();
        public Dictionary<string, GameObject> Targets = new Dictionary<string, GameObject>();

        //private Client _client;
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
            _cam = Camera.main;

#if UNITY_EDITOR
            InitServer();
#else
            InitPlayer();
            InitClient();
#endif
        }

        private void InitServer()
        {
            Instantiate(Resources.Load(CONSTANTS.ServerPrefab));
            _cam.transform.localPosition = CONSTANTS.MyShooterServerCamOffset;
            _cam.transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        private void InitPlayer()
        {
            GameObject go = Instantiate(Resources.Load(CONSTANTS.PlayerPrefab), new Vector3(0, 0.5f, 0), Quaternion.identity) as GameObject;
            LocalPlayer = go.GetComponent<Player>();
            LocalPlayer.IsLocal = true;

            _cam.transform.SetParent(go.transform);
            _cam.transform.localPosition = CONSTANTS.MyShooterCameraOffset;
        }

        private void InitClient()
        {
            GameObject go = Instantiate(Resources.Load(CONSTANTS.ClientPrefab)) as GameObject;
            go.GetComponent<ClientInit>().ConnectClient();
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
                CanvasController.Instance.QueueButton.interactable = false;
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
            else if (msg is BulletSpawn)
            {
                BulletSpawn bs = msg as BulletSpawn;
                Vector3 pos = new Vector3(bs.Position.X, bs.Position.Y, bs.Position.Z);
                Vector3 dir = new Vector3(bs.Direction.X, bs.Direction.Y, bs.Direction.Z);
                GameObject go = Instantiate(Resources.Load(CONSTANTS.ProjectilePrefab), pos, Quaternion.identity) as GameObject;
                go.GetComponent<Rigidbody>().velocity = dir * CONSTANTS.ProjectileSpeed;
            }
            else if (msg is TargetSpawn)
            {
                TargetSpawn ts = msg as TargetSpawn;
                Vector3 pos = new Vector3(ts.Position.X, ts.Position.Y, ts.Position.Z);
                GameObject go = Instantiate(Resources.Load(CONSTANTS.TargetPrefab)) as GameObject;
                go.transform.position = pos;
                Targets.Add(ts.Id, go);
            }
            else if (msg is BulletCollision)
            {
                BulletCollision bc = msg as BulletCollision;
                if (Targets.ContainsKey(bc.Id))
                {
                    Destroy(Targets[bc.Id]);
                    Targets.Remove(bc.Id);
                }
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
                        Player p = Players[pos.Username].GetComponent<Player>();
                        p.LerpMovement(new Vector3(pos.X, pos.Y, pos.Z));
                        p.LerpRotation(pos.YRotation);
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