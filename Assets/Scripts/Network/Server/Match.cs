using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DTOs;
using GameClient;

public class Match
{
    public bool IsLive = true;
    public List<Client> Clients = new List<Client>();
    private ConcurrentDictionary<string, Message> _playerPositions = new ConcurrentDictionary<string, Message>();

    public Match()
    {
    }

    public void AddPlayer(Client client)
    {
        Clients.Add(client);
    }

    public void RemovePlayer(Client client)
    {
        Clients.Remove(client);
    }

    public Message[] GetState()
    {
        return _playerPositions.Values.ToArray();
    }

    public void UpdateState()
    {
        foreach (Client client in Clients)
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