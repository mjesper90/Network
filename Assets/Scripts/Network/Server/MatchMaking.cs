using System;
using System.Collections.Concurrent;
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
        public ConcurrentQueue<Client> Queue = new ConcurrentQueue<Client>();

        private bool _matchGoing = false;
        private int _matchMinPlayers = 1;

        public MatchMaking(IMatch match)
        {
            Matches = new List<IMatch>() { match };
            match.StartUpdateLoop();
            StartQueueLoop();
        }

        public virtual void Join(Client client)
        {
            Queue.Enqueue(client);
            client.NetworkHandler.InQueue = true;
            _ = client.SendAsync(new Message(MessageType.QueueJoined));
        }

        public void Shutdown()
        {
            foreach (IMatch match in Matches)
            {
                match.Shutdown();
            }
        }

        private void StartQueueLoop()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    if (Queue.Count >= _matchMinPlayers && !_matchGoing)
                    {
                        _matchGoing = true;
                        Server.Log.Log("Match going");
                    }
                    if (Queue.Count > 0 && _matchGoing)
                    {
                        Client client = null;
                        Queue.TryDequeue(out client);
                        _ = Matches.First().AddPlayer(client);
                        client.NetworkHandler.InQueue = false;
                        client.NetworkHandler.InGame = true;
                    }
                    await Task.Delay((int)(1000 * CONSTANTS.ServerSpeed));
                }
            });
        }
    }
}
