using System;
using System.Collections.Generic;
using System.Threading;
using MyShooter.DTOs;
using MyShooter.Logging;
using NetworkLib.Common.DTOs;
using NetworkLib.Common.DTOs.MatchMaking;
using NetworkLib.GameServer;
using UnityEngine;

namespace MyShooter.NetworkSetup
{
    public class ServerInit : MonoBehaviour
    {
        public Server Server;
        public List<Player> Players;
        private ShooterMatch _match;

        public static ServerInit Instance;

        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                _match = new ShooterMatch();
                Server = new Server(new UnityLogger("Server::"), CONSTANTS.Port, _match);
                Server.StartAcceptingClients();
                Debug.Log("Server started");
                Instance = this;
                Players = new List<Player>();
                //Spawn a couple of targets in the match
                for (int i = 0; i < 10; i++)
                {
                    GameObject go = Instantiate(Resources.Load(CONSTANTS.TargetPrefab)) as GameObject;
                    go.transform.position = new Vector3(UnityEngine.Random.Range(-50, 50), 0.5f, UnityEngine.Random.Range(-50, 50));
                    go.name = "Target " + i;
                    Target t = go.GetComponent<Target>();
                    TargetSpawn ts = new TargetSpawn(go.transform.position.x, go.transform.position.y, go.transform.position.z);
                    t.TargetSpawn = ts;
                    _match.Targets.Add(ts.Id, ts);
                }
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
                GameObject go = GameObject.Instantiate(Resources.Load(CONSTANTS.MyShooterPlayerPrefab), new Vector3(0, 0.5f, 0), Quaternion.identity) as GameObject;
                Player p = go.GetComponent<Player>();
                Players.Add(p);
                p.Username = pj.Username;
                p.MatchId = pj.MatchId;
                p.LoggedIn = true;
            }
            else if (msg is BulletSpawn)
            {
                BulletSpawn bs = msg as BulletSpawn;
                GameObject go = GameObject.Instantiate(Resources.Load(CONSTANTS.ProjectilePrefab), new Vector3(bs.Position.X, bs.Position.Y, bs.Position.Z), Quaternion.identity) as GameObject;
                MonoProjectile mp = go.GetComponent<MonoProjectile>();
                mp.BulletSpawn = bs;
                go.GetComponent<Rigidbody>().velocity = new Vector3(bs.Direction.X, bs.Direction.Y, bs.Direction.Z) * CONSTANTS.ProjectileSpeed;
                mp.OnCollision += HandleCollision;
            }
            else if (msg is PlayerLeft)
            {
                PlayerLeft pl = msg as PlayerLeft;
                Players.Remove(Players.Find(p => p.Username == pl.Username));
            }
        }

        private void HandleCollision(Collision col, MonoProjectile mp)
        {
            Debug.Log(mp.BulletSpawn.Id + " collided with " + col.gameObject.name);
            if (col.gameObject.GetComponent<Target>() != null)
            {
                _ = _match.Broadcast(new BulletCollision(col.gameObject.GetComponent<Target>().TargetSpawn.Id));
                // Remove the target from the match
                Destroy(col.gameObject);
            }
            // Destroy the projectile
            Destroy(mp.gameObject);
        }

        void OnApplicationQuit()
        {
            if (Server != null)
                Server.Shutdown();
            Debug.Log("Server stopped");
        }
    }
}