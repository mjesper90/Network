using System.Collections.Concurrent;
using System.Linq;
using NetworkLib.Common.DTOs;
using NetworkLib.GameClient;

namespace NetworkLib.GameServer
{
    public class Match : IMatch
    {
        protected ConcurrentDictionary<string, Client> Clients = new ConcurrentDictionary<string, Client>();

        public Match()
        {
        }

        public virtual Client[] GetClients()
        {
            return Clients.Values.ToArray();
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
            return new Message[] { };
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
                            case MessageType.Message:
                                Server.Log.Log($"Match handling Message {msg.MsgType}");
                                break;
                            default:
                                Server.Log.Log($"Unhandled message type {msg.MsgType}");
                                break;
                        }
                    }
                }
            }
        }

        public virtual void Broadcast(Message msg)
        {
            foreach (Client client in Clients.Values)
            {
                _ = client.SendAsync(msg);
            }
        }
    }
}