using System.Collections.Generic;
using NetworkLib.GameClient;

namespace NetworkLib.GameServer
{
    public class MatchMaking
    {
        public List<Match> Matches = new List<Match>();

        public MatchMaking()
        {
            Matches.Add(new Match());
        }

        public void UpdateMatches()
        {
            foreach (Match match in Matches)
            {
                match.UpdateState();
                Message[] messages = match.GetState();
                foreach (Client client in match.Clients.Values)
                {
                    _ = client.SendAsync(messages);
                }
            }
        }

        public void Join(Client client)
        {
            Matches[0].AddPlayer(client);
            client.Send(new Message(MessageType.MatchJoined, new byte[0], ""));
            UnityEngine.Debug.Log($"User {client.NetworkHandler.User.Username} joined match");
            client.NetworkHandler.InGame = true;
            client.NetworkHandler.InQueue = false;
        }
    }
}