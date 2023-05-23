using System.Collections.Concurrent;
using System.Linq;
using NetworkLib.Common.DTOs;
using NetworkLib.GameClient;

namespace NetworkLib.GameServer
{
    public class Match
    {
        public ConcurrentDictionary<string, Client> Clients = new ConcurrentDictionary<string, Client>();
        protected ConcurrentDictionary<string, Message> _playerPositions = new ConcurrentDictionary<string, Message>();

        public Match()
        {
        }

        public virtual void AddPlayer(Client client)
        {
            Clients.TryAdd(client.NetworkHandler.Auth.Username, client);

            // Notify other clients in the match about the new player
            foreach (Client c in Clients.Values)
            {
                if (c.NetworkHandler.Auth.Username != client.NetworkHandler.Auth.Username)
                {
                    _ = c.SendAsync(new Message(MessageType.PlayerJoined, c.Serialize(client.NetworkHandler.Auth.Username)));
                }
            }
        }

        public virtual void RemovePlayer(Client client)
        {
            Clients.TryRemove(client.NetworkHandler.Auth.Username, out Client removedClient);

            // Notify other clients in the match about the removed player
            foreach (Client c in Clients.Values)
            {
                if (c.NetworkHandler.Auth.Username != client.NetworkHandler.Auth.Username)
                {
                    _ = c.SendAsync(new Message(MessageType.PlayerLeft, c.Serialize(client.NetworkHandler.Auth.Username)));
                }
            }
        }

        public virtual Message[] GetState()
        {
            return _playerPositions.Values.ToArray();
        }

        public virtual void UpdateState()
        {
            foreach (Client client in Clients.Values)
            {
                if (client.NetworkHandler.Auth == null) //Player not logged in?
                {
                    continue;
                }
                while (client.NetworkHandler.GetQueueSize() > 0)
                {
                    if (client.NetworkHandler.TryDequeue(out Message msg))
                    {
                        switch (msg.MsgType)
                        {
                            case MessageType.User:
                                //client.NetworkHandler.User = client.Deserialize<User>(msg.Data);
                                Server.Log.Log("Match handling User" + client.NetworkHandler.Auth);
                                break;
                            case MessageType.PlayerPosition:
                                _playerPositions[client.NetworkHandler.Auth.Username] = msg;
                                Server.Log.Log("Match handling PlayerPosition " + client.NetworkHandler.Auth.Username);
                                break;
                            default:
                                Server.Log.Log($"Unhandled message type {msg.MsgType}");
                                break;
                        }
                    }
                }
            }
        }
    
    }
}