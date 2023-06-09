using System.Collections.Concurrent;
using System.Threading.Tasks;
using MyGame.DTOs;
using NetworkLib.Common.DTOs;
using NetworkLib.Common.DTOs.MatchMaking;
using NetworkLib.GameClient;
using NetworkLib.GameServer;

public class NotQuakeMatch : Match
{
    protected ConcurrentDictionary<string, Message> _bullets = new ConcurrentDictionary<string, Message>();
    protected ConcurrentDictionary<string, PositionAndYRotation> _playerPositions = new ConcurrentDictionary<string, PositionAndYRotation>();

    public override Message[] GetState()
    {
        Message[] messages = new Message[_playerPositions.Count + _bullets.Count];
        int i = 0;
        foreach (Message msg in _playerPositions.Values)
        {
            messages[i++] = msg;
        }
        foreach (Message msg in _bullets.Values)
        {
            messages[i++] = msg;
        }
        _playerPositions.Clear();
        _bullets.Clear();

        return messages;
    }

    protected override async Task ProcessMessage(Message msg, Client client)
    {
        //Server.Log.Log($"NotQuakeMatch processing message type {msg.GetType().Name} from {client.NetworkHandler.Auth.Username}");
        string username = client.NetworkHandler.Auth.Username;
        if (msg is PositionAndYRotation)
        {
            _playerPositions[username] = msg as PositionAndYRotation;
        }
    }

    protected override async Task UpdateState()
    {
        await base.UpdateState();
        //Move bullets
    }
}