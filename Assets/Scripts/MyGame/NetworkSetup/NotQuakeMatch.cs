using System.Collections.Concurrent;
using System.Threading.Tasks;
using NetworkLib.Common.DTOs;
using NetworkLib.GameClient;
using NetworkLib.GameServer;

public class NotQuakeMatch : Match
{
    protected ConcurrentDictionary<string, Message> _bullets = new ConcurrentDictionary<string, Message>();
    protected ConcurrentDictionary<string, Message> _playerPositions = new ConcurrentDictionary<string, Message>();
    protected ConcurrentDictionary<string, Message> _playerRotations = new ConcurrentDictionary<string, Message>();

    public override Message[] GetState()
    {
        Message[] messages = new Message[_playerPositions.Count + _playerRotations.Count + _bullets.Count];
        int i = 0;
        foreach (Message msg in _playerPositions.Values)
        {
            messages[i++] = msg;
        }
        foreach (Message msg in _playerRotations.Values)
        {
            messages[i++] = msg;
        }
        foreach (Message msg in _bullets.Values)
        {
            messages[i++] = msg;
        }
        _playerPositions.Clear();
        _playerRotations.Clear();
        _bullets.Clear();

        return messages;
    }

    protected override async Task ProcessMessage(Message msg, Client client)
    {
        //Server.Log.Log($"NotQuakeMatch processing message type {msg.MsgType}");
        string username = client.NetworkHandler.Auth.Username;
        switch (msg.MsgType)
        {
            case MessageType.PlayerPosition:
                _playerPositions[username] = msg;
                Server.Log.Log("NotQuakeMatch handling PlayerPosition " + username);
                break;
            case MessageType.PlayerRotation:
                _playerRotations[username] = msg;
                Server.Log.Log("NotQuakeMatch handling PlayerRotation " + username);
                break;
            case MessageType.Shoot:
                _bullets[msg.Id] = msg;
                Server.Log.Log("NotQuakeMatch handling Bullet " + msg.Id);
                break;
            case MessageType.Message:
                Server.Log.Log("NotQuakeMatch handling Message " + msg.Id);
                break;
            default:
                Server.Log.Log($"NotQuakeMatch Unhandled message type {msg.MsgType}");
                break;
        }
    }

}