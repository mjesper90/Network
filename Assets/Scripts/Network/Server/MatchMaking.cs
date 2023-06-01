using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetworkLib.Common.DTOs;
using NetworkLib.Common.Interfaces;
using NetworkLib.GameClient;

namespace NetworkLib.GameServer
{
    public class MatchMaking : IMatchMaking
    {
        public ICollection<IMatch> Matches;

        public MatchMaking(IMatch match)
        {
            Matches = new List<IMatch>() { match };
            match.StartUpdateLoop();
        }
        public virtual async Task Join(Client client)
        {
            IMatch match = Matches.First(); // Assuming only one match is available
            _ = match.AddPlayer(client);
            client.SetMatch(match);
            await client.SendAsync(new Message(MessageType.MatchJoined));
            Server.Log.Log($"User {client.NetworkHandler.Auth.Username} joined match");
            client.NetworkHandler.InGame = true;
            client.NetworkHandler.InQueue = false;
        }
    }
}
