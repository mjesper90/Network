using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmongUs.DTOs;
using NetworkLib.Common.DTOs;
using NetworkLib.Common.DTOs.MatchMaking;
using NetworkLib.GameClient;
using NetworkLib.GameServer;

namespace AmongUs
{
    public class AmongUsMatch : Match
    {
        public ConcurrentQueue<Message> UnityMessages = new ConcurrentQueue<Message>();
        public ConcurrentDictionary<string, PositionAndYRotation> PlayerPositions = new ConcurrentDictionary<string, PositionAndYRotation>();

        public override async Task AddPlayer(Client client)
        {
            await base.AddPlayer(client);

            UnityMessages.Enqueue(new PlayerJoined(client.NetworkHandler.Auth.Username, _id.ToString()));
        }

        public override async Task RemovePlayer(Client client)
        {
            await base.RemovePlayer(client);

            UnityMessages.Enqueue(new PlayerLeft(client.NetworkHandler.Auth.Username, _id.ToString()));
            PlayerPositions.TryRemove(client.NetworkHandler.Auth.Username, out _);
        }

        public override Message[] GetState()
        {
            List<Message> messages = new List<Message>();
            foreach (KeyValuePair<string, PositionAndYRotation> kvp in PlayerPositions)
            {
                messages.Add(kvp.Value);
            }
            return messages.ToArray();
        }

        protected override async Task ProcessMessage(Message msg, Client client)
        {
            //Server.Log.Log($"AmongUsMatch processing message type {msg.GetType().Name} from {client.NetworkHandler.Auth.Username}");
            string username = client.NetworkHandler.Auth.Username;
            if (msg is PositionAndYRotation) // Position and rotation update
            {
                PlayerPositions[username] = msg as PositionAndYRotation;
            }
            else
            {
                await base.ProcessMessage(msg, client);
            }
        }
    }
}