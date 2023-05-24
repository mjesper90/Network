using System.Collections.Concurrent;
using System.Linq;
using NetworkLib.Common.DTOs;
using NetworkLib.GameClient;
using NetworkLib.GameServer;

public class NotQuakeMatch : Match
{
    protected ConcurrentDictionary<string, Message> _playerPositions = new ConcurrentDictionary<string, Message>();
    protected ConcurrentDictionary<string, Message> _playerRotations = new ConcurrentDictionary<string, Message>();

    public override Message[] GetState()
    {
        Message[] messages = new Message[_playerPositions.Count + _playerRotations.Count];
        int i = 0;
        foreach (Message msg in _playerPositions.Values)
        {
            messages[i++] = msg;
        }
        foreach (Message msg in _playerRotations.Values)
        {
            messages[i++] = msg;
        }
        return messages;
    }

    public override void UpdateState()
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
                        case MessageType.PlayerPosition:
                            _playerPositions[client.NetworkHandler.Auth.Username] = msg;
                            Server.Log.Log("NotQuakeMatch handling PlayerPosition " + client.NetworkHandler.Auth.Username);
                            break;
                        case MessageType.PlayerRotation:
                            _playerRotations[client.NetworkHandler.Auth.Username] = msg;
                            Server.Log.Log("NotQuakeMatch handling PlayerRotation " + client.NetworkHandler.Auth.Username);
                            break;
                        default:
                            Server.Log.Log($"NotQuakeMatch Unhandled message type {msg.MsgType}");
                            break;
                    }
                }
            }
        }
    }

}