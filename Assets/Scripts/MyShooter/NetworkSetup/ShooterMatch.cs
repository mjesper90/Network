using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyShooter.DTOs;
using NetworkLib.Common.DTOs;
using NetworkLib.Common.DTOs.MatchMaking;
using NetworkLib.GameClient;
using NetworkLib.GameServer;

namespace MyShooter
{
    public class ShooterMatch : Match
    {
        public Dictionary<string, TargetSpawn> Targets = new Dictionary<string, TargetSpawn>();
        public ConcurrentQueue<Message> UnityMessages = new ConcurrentQueue<Message>();
        public ConcurrentDictionary<string, BulletSpawn> Bullets = new ConcurrentDictionary<string, BulletSpawn>();
        public ConcurrentDictionary<string, PositionAndYRotation> PlayerPositions = new ConcurrentDictionary<string, PositionAndYRotation>();

        public override async Task AddPlayer(Client client)
        {
            await base.AddPlayer(client);

            UnityMessages.Enqueue(new PlayerJoined(client.NetworkHandler.Auth.Username, _id.ToString()));
            //Send TargetSpawns
            await client.SendAsync(Targets.Values.ToArray());
        }

        public override Message[] GetState()
        {
            List<Message> messages = new List<Message>();
            messages.AddRange(PlayerPositions.Values);
            return messages.ToArray();
        }

        protected override async Task ProcessMessage(Message msg, Client client)
        {
            Server.Log.Log($"ShooterMatch processing message type {msg.GetType().Name} from {client.NetworkHandler.Auth.Username}");
            string username = client.NetworkHandler.Auth.Username;
            if (msg is PositionAndYRotation) // Position and rotation update
            {
                PlayerPositions[username] = msg as PositionAndYRotation;
            }
            else if (msg is BulletSpawn) // New bullet spawned
            {
                UnityMessages.Enqueue(msg);
                _ = Broadcast(msg);
            }
            else
            {
                await base.ProcessMessage(msg, client);
            }
        }
    }
}