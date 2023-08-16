using System;
using System.Collections.Generic;
using System.Threading;
using AmongUs.DTOs;
using AmongUs.Logging;
using NetworkLib.Common.DTOs;
using NetworkLib.Common.DTOs.MatchMaking;
using NetworkLib.GameServer;
using UnityEngine;

namespace AmongUs.NetworkSetup
{
    public class ServerInit : MonoBehaviour
    {
        public Server Server;
        public List<Player> Players;
        private AmongUsMatch _match;

        public static ServerInit Instance;

        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                _match = new AmongUsMatch();
                Players = new List<Player>();
                Server = new Server(new UnityLogger("Server::"), CONSTANTS.Port, _match);
                Server.StartAcceptingClients();
                Debug.Log("Server started");
                Instance = this;
            }
        }

        private void Update()
        {
            // Is there a match, and are there any new messages?
            while (_match?.UnityMessages.Count > 0)
            {
                // Try to get a message
                if (_match.UnityMessages.TryDequeue(out Message msg))
                {
                    // Handle the message
                    HandleMessage(msg);
                }
            }
        }

        public void FixedUpdate()
        {
            // Update player positions from match
            foreach (Player p in Players)
            {
                if (_match.PlayerPositions.TryGetValue(p.Username, out PositionAndYRotation pos))
                {
                    p.transform.position = new Vector3(pos.X, pos.Y, pos.Z);
                    p.transform.rotation = Quaternion.Euler(0, pos.YRotation, 0);
                }
            }
        }

        private void HandleMessage(Message msg)
        {
            if (msg is PlayerJoined)
            {
                PlayerJoined pj = msg as PlayerJoined;
                Debug.Log($"Player {pj.Username} joined");
                GameObject go = GameObject.Instantiate(Resources.Load(CONSTANTS.AmongUsPlayerPrefab), new Vector3(0, 0.5f, 0), Quaternion.identity) as GameObject;
                Player p = go.GetComponent<Player>();
                Players.Add(p);
                p.Username = pj.Username;
                p.MatchId = pj.MatchId;
                p.LoggedIn = true;
            }
            if (msg is PlayerLeft)
            {
                PlayerLeft pl = msg as PlayerLeft;
                Debug.Log($"Player {pl.Username} left");
                Player p = Players.Find(x => x.Username == pl.Username);
                if (p != null)
                {
                    Players.Remove(p);
                    Destroy(p.gameObject);
                }
            }
        }

        void OnApplicationQuit()
        {
            if (Server != null)
                Server.Shutdown();
            Debug.Log("Server stopped");
        }
    }
}