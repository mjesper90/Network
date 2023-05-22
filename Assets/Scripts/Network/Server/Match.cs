using System.Collections.Concurrent;
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
            Clients.TryAdd(client.NetworkHandler.Conn.Username, client);

            // Notify other clients in the match about the new player
            foreach (Client c in Clients.Values)
            {
                if (c.NetworkHandler.Conn.Username != client.NetworkHandler.Conn.Username)
                {
                    _ = c.SendAsync(new Message(MessageType.PlayerJoined, c.Serialize(client.NetworkHandler.Conn.Username), ""));
                }
            }
        }

        public void RemovePlayer(Client client)
        {
            Clients.TryRemove(client.NetworkHandler.Conn.Username, out Client removedClient);

            // Notify other clients in the match about the removed player
            foreach (Client c in Clients.Values)
            {
                if (c.NetworkHandler.Conn.Username != client.NetworkHandler.Conn.Username)
                {
                    _ = c.SendAsync(new Message(MessageType.PlayerLeft, c.Serialize(client.NetworkHandler.Conn.Username), ""));
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
                if (client.NetworkHandler.Conn == null) //Player not logged in?
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
                                UnityEngine.Debug.Log("Match handling User" + client.NetworkHandler.Conn);
                                break;
                            case MessageType.PlayerPosition:
                                _playerPositions[client.NetworkHandler.Conn.Username] = msg;
                                UnityEngine.Debug.Log("Match handling PlayerPosition " + client.NetworkHandler.Conn.Username);
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