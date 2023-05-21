using System.Collections.Concurrent;
using DTOs;

namespace GameClient
{
    public class Network
    {
        public Client Client;
        public User User = null;
        public bool InGame = false;
        public bool InQueue = false;

        public ConcurrentQueue<Message> MessageQueue = new ConcurrentQueue<Message>();

        public Network(Client client)
        {
            Client = client;
        }
    }
}