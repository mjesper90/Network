using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetworkLib.Common.DTOs;
using NetworkLib.Common.DTOs.MatchMaking;
using NetworkLib.Common.Interfaces;
using NetworkLib.GameClient;

namespace NetworkLib.GameServer
{
    public class MatchMaking
    {
        public ICollection<IMatch> Matches;
        public ConcurrentQueue<Client> Queue = new ConcurrentQueue<Client>();

        protected bool _matchGoing = false;
        protected int _matchMinPlayers = 1;

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
            _ = client.SendAsync(new QueueMessage());
        }

        public void Shutdown()
        {
            foreach (IMatch match in Matches)
            {
                match.StopUpdateLoop();
            }
        }

        protected virtual void StartQueueLoop()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    if (Queue.Count >= _matchMinPlayers && !_matchGoing)
                    {
                        _matchGoing = true;
                    }
                    if (Queue.Count > 0 && _matchGoing) //Join players to match
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
