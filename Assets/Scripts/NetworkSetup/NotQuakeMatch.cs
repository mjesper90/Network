using System.Linq;
using NetworkLib.Common.DTOs;
using NetworkLib.GameClient;
using NetworkLib.GameServer;

public class NotQuakeMatch : Match
{
    public override Message[] GetState()
    {
        return _playerPositions.Values.ToArray();
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

    private void Broadcast(Message msg)
    {
        foreach (Client c in Clients.Values)
        {
            _ = c.SendAsync(msg);
        }
    }
}