using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NetworkLib.GameClient;

namespace NetworkLib.GameServer
{
    public class Match
    {
        public ConcurrentDictionary<string, Client> Clients = new ConcurrentDictionary<string, Client>();
        private ConcurrentDictionary<string, Message> _playerPositions = new ConcurrentDictionary<string, Message>();

        public Match()
        {
        }

        public void AddPlayer(Client client)
        {
            Clients.TryAdd(client.NetworkHandler.User.Username, client);

            // Notify other clients in the match about the new player
            foreach (Client c in Clients.Values)
            {
                if (c.NetworkHandler.User.Username != client.NetworkHandler.User.Username)
                {
                    _ = c.SendAsync(new Message(MessageType.PlayerJoined, c.Serialize(client.NetworkHandler.User.Username), ""));
                }
            }
        }

        public void RemovePlayer(Client client)
        {
            Clients.TryRemove(client.NetworkHandler.User.Username, out Client removedClient);

            // Notify other clients in the match about the removed player
            foreach (Client c in Clients.Values)
            {
                if (c.NetworkHandler.User.Username != client.NetworkHandler.User.Username)
                {
                    _ = c.SendAsync(new Message(MessageType.PlayerLeft, c.Serialize(client.NetworkHandler.User.Username), ""));
                }
            }
        }

        public Message[] GetState()
        {
            return _playerPositions.Values.ToArray();
        }

        public void UpdateState()
        {
            foreach (Client client in Clients.Values)
            {
                if (client.NetworkHandler.User == null) //Player not logged in?
                {
                    continue;
                }
                while (client.NetworkHandler.MessageQueue.Count > 0)
                {
                    if (client.NetworkHandler.MessageQueue.TryDequeue(out Message msg))
                    {
                        switch (msg.MsgType)
                        {
                            case MessageType.User:
                                //client.NetworkHandler.User = client.Deserialize<User>(msg.Data);
                                UnityEngine.Debug.Log("Match handling User" + client.NetworkHandler.User);
                                break;
                            case MessageType.PlayerPosition:
                                _playerPositions[client.NetworkHandler.User.Username] = msg;
                                UnityEngine.Debug.Log("Match handling PlayerPosition " + client.NetworkHandler.User.Username);
                                break;
                            default:
                                UnityEngine.Debug.Log($"Unhandled message type {msg.MsgType}");
                                break;
                        }
                    }
                }
            }
        }
    }
}